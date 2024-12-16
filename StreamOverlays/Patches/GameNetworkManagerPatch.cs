using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal static class GameNetworkManagerPatch
{
    [HarmonyPatch(nameof(GameNetworkManager.SaveGame))]
    [HarmonyPostfix]
    private static void SaveGamePatch()
    {
        StatsHelper.SaveDayData();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(GameNetworkManager.ResetSavedGameValues))]
    [HarmonyPostfix]
    private static void ResetSavedGameValuesPatch()
    {
        StatsHelper.ResetSavedDayData();
        WebServer.UpdateOverlaysData();
    }
}
