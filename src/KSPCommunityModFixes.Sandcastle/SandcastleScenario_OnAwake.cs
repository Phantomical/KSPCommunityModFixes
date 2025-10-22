using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Sandcastle;
using Sandcastle.Inventory;

namespace KSPCommunityModFixes.Sandcastle;

/// <summary>
/// <c>InventoryUtils.FindThumbnailPaths</c> scans the entirety of GameData.
/// The list it computes from this is never used anywhere so we just delete
/// the one call to it.
/// </summary>
[HarmonyPatch(typeof(SandcastleScenario), nameof(SandcastleScenario.OnAwake))]
static class SandcastleScenario_OnAwake_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator gen
    )
    {
        var method = Tools.GetMethodInfo(() => InventoryUtils.FindThumbnailPaths());

        var matcher = new CodeMatcher(instructions, gen);
        matcher.Repeat(matcher =>
        {
            matcher.MatchStartForward(new CodeMatch(OpCodes.Call, method)).RemoveInstruction();
        });

        return matcher.Instructions();
    }
}
