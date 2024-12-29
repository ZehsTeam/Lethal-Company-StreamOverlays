using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal static class TimeOfDayPatch
{
    [HarmonyPatch(nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
    [HarmonyPostfix]
    private static void SyncNewProfitQuotaClientRpcPatch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(TimeOfDay.UpdateProfitQuotaCurrentTime))]
    [HarmonyPostfix]
    private static void UpdateProfitQuotaCurrentTimePatch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }
}
