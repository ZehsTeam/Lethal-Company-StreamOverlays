using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;
using ShipInventory.Helpers;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;

[HarmonyPatch(typeof(ItemManager))]
internal static class ItemManagerPatch
{
    [HarmonyPatch(nameof(ItemManager.UpdateCache))]
    [HarmonyPostfix]
    private static void UpdateCachePatch()
    {
        if (LootManager.CanUpdateLootTotal())
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }
}
