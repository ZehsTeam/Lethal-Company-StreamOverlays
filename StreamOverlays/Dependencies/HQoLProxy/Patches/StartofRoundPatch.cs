using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Dependencies.HQoLProxy.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    private static void StartGamePatch()
    {
        HQoLProxy.UpdateStorageValueAtStartOfRound();
    }
}
