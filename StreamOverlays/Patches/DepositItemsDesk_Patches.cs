using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(DepositItemsDesk))]
internal static class DepositItemsDesk_Patches
{
    [HarmonyPatch(nameof(DepositItemsDesk.SellAndDisplayItemProfits))]
    [HarmonyPostfix]
    private static void SellAndDisplayItemProfits_Patch()
    {
        if (LootManager.CanUpdateLootTotal())
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }
}
