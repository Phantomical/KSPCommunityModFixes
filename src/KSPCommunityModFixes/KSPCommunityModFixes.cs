using System.Reflection;
using HarmonyLib;
using ModuleManager;
using UnityEngine;

namespace KSPCommunityModFixes;

[KSPAddon(KSPAddon.Startup.Instantly, once: true)]
internal class KSPCommunityModFixes : MonoBehaviour
{
    readonly Harmony harmony = new("KSPCommunityModFixes");

    void Awake()
    {
        // Explicitly added post patch callbacks are run before MM attempts to
        // run any static post-patch callback methods. This is early enough for
        // us.
        PostPatchLoader.AddPostPatchCallback(OnMMPostPatch);
    }

    void OnMMPostPatch()
    {
        var configs = GameDatabase.Instance.GetConfigs("KSPCMF_CONFIG");
        var config = configs?[0]?.config ?? new ConfigNode();

        foreach (var loadedAssembly in AssemblyLoader.loadedAssemblies)
        {
            var assembly = loadedAssembly.assembly;
            var attribute = assembly?.GetCustomAttribute<KSPCommunityModFixesAssemblyAttribute>();
            if (attribute is null)
                continue;

            var name = loadedAssembly.name;
            if (!name.StartsWith("KSPCommunityModFixes."))
                continue;
            var subname = name.Substring("KSPCommunityModFixes.".Length);

            bool enabled = false;
            config.TryGetValue(subname, ref enabled);

            if (enabled)
            {
                Debug.Log($"[KSPCommunityModFixes] Applying patches for {subname}");
                harmony.PatchAll(assembly);
            }
            else
            {
                Debug.Log($"[KSPCommunityModFixes] Patches for {subname} disabled by config");
            }
        }
    }
}
