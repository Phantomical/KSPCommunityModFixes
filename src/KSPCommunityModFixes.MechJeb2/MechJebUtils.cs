using HarmonyLib;
using MuMech;

namespace KSPCommunityModFixes.MechJeb2;

internal static class MechJebUtils
{
    // TODO: There has got to be a better way to figure this out.
    internal static bool IsDevBranch() =>
        AccessTools.Field(typeof(ValueInfoItem), "_getterCache") is null;
}
