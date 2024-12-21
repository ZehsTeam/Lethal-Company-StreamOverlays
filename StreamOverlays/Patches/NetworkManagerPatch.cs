using HarmonyLib;
using Unity.Netcode;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(NetworkManager))]
internal static class NetworkManagerPatch
{
    [HarmonyPatch("Initialize")]
    [HarmonyPostfix]
    private static void InitializePatch()
    {
        PluginNetworkManager.Initialize();
    }
}
