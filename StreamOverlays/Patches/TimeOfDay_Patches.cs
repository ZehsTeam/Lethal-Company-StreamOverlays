using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal static class TimeOfDay_Patches
{
    [HarmonyPatch(nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
    [HarmonyPostfix]
    private static void SyncNewProfitQuotaClientRpc_Patch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(TimeOfDay.UpdateProfitQuotaCurrentTime))]
    [HarmonyPostfix]
    private static void UpdateProfitQuotaCurrentTime_Patch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }
}
