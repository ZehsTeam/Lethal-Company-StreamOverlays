using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal static class TimeOfDayPatch
{
    [HarmonyPatch(nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
    [HarmonyPostfix]
    private static void SyncNewProfitQuotaClientRpcPatch()
    {
        WebSocketClient.UpdateOverlay();
    }
}
