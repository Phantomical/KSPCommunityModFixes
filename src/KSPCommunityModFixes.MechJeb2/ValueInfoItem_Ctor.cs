using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MuMech;

namespace KSPCommunityModFixes.MechJeb2;

/// <summary>
/// ValueInfoItem compiles a lambda in order to access the target field.
/// Unfortunately, this is uncached and compiling the lambda takes quite
/// a while, making scene switches slow.
///
/// This patch adds a cache that can be used instead.
/// </summary>
[HarmonyPatch(typeof(ValueInfoItem), MethodType.Constructor)]
internal static class ValueInfoItem_Ctor
{
    static readonly Dictionary<MemberInfo, Func<object, object>> GetterCache = [];

    // Disable this patch if on the mechjeb dev version.
    //
    // There doesn't seem to be a better way to track this since all the
    // versions appear to be identical in each case.
    static bool Prepare() => AccessTools.Field(typeof(ValueInfoItem), "_getterCache") is null;

    static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator gen
    )
    {
        var timeDecimalPlaces = AccessTools.Field(typeof(ValueInfoItem), "timeDecimalPlaces");
        var getValue = AccessTools.Field(typeof(ValueInfoItem), "getValue");
        var matcher = new CodeMatcher(instructions, gen);

        matcher
            .MatchStartForward(new CodeMatch(OpCodes.Stfld, timeDecimalPlaces))
            .ThrowIfInvalid("Unable to find store to timeDecimalPlaces field")
            .Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(
                    OpCodes.Call,
                    Tools.GetMethodInfo(() => GetMemberAccessor(null, null))
                ),
                new CodeInstruction(OpCodes.Stfld, getValue),
                new CodeInstruction(OpCodes.Ret)
            );

        return matcher.Instructions();
    }

    static Func<object> GetMemberAccessor(object obj, MemberInfo member)
    {
        if (!GetterCache.TryGetValue(member, out var getter))
        {
            ParameterExpression objExpr = Expression.Parameter(typeof(object));
            Expression castExpr = Expression.Convert(objExpr, obj.GetType());
            Expression memberExpr;
            if (member is MethodInfo method)
                memberExpr = Expression.Call(castExpr, method);
            else
                memberExpr = Expression.MakeMemberAccess(castExpr, member);
            Expression castMemberExpr = Expression.Convert(memberExpr, typeof(object));
            getter = Expression.Lambda<Func<object, object>>(castMemberExpr, objExpr).Compile();

            GetterCache[member] = getter;
        }

        return () => getter(obj);
    }
}
