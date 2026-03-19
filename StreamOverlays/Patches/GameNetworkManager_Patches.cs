using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal static class GameNetworkManager_Patches
{
    [HarmonyPatch(nameof(GameNetworkManager.SaveGame))]
    [HarmonyPostfix]
    private static void SaveGame_Patch()
    {
        DayManager.SaveDayData();
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(GameNetworkManager.ResetSavedGameValues))]
    [HarmonyPostfix]
    private static void ResetSavedGameValues_Patch()
    {
        DayManager.ResetSavedDayData();
        WebServer.UpdateOverlaysData();

        Logger.LogInfo("Reset saved data.", extended: true);
    }
}
