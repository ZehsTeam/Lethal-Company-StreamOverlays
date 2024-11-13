using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(DepositItemsDesk))]
internal static class DepositItemsDeskPatch
{
    [HarmonyPatch(nameof(DepositItemsDesk.SellAndDisplayItemProfits))]
    [HarmonyPostfix]
    private static void SellAndDisplayItemProfitsPatch()
    {
        WebSocketClient.UpdateOverlay(); // Update Loot
    }
}
