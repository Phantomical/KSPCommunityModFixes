using System;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;

namespace KSPCommunityModFixes;

/// <summary>
/// Helper methods for doing common patch tasks.
/// </summary>
public static class Tools
{
    /// <summary>
    /// Extract the method info from a lambda that makes a call to a function.
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static MethodInfo GetMethodInfo(LambdaExpression expr) =>
        SymbolExtensions.GetMethodInfo(expr);

    /// <summary>
    /// Extract the method info from a lambda that makes a call to a function.
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expr) =>
        SymbolExtensions.GetMethodInfo(expr);

    public static FieldInfo GetFieldInfo(LambdaExpression expr)
    {
        if (expr.Body is not MemberExpression member)
            throw new ArgumentException("expected a member expression");

        if (member.Member is not FieldInfo field)
            throw new ArgumentException("expected a field access expression");

        return field;
    }

    public static FieldInfo GetFieldInfo<T>(Expression<Action<T>> expr)
    {
        if (expr is not LambdaExpression lambda)
            throw new ArgumentException("expected a lambda expression");

        return GetFieldInfo(lambda);
    }
}
