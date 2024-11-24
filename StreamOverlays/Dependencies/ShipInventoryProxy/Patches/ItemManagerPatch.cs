using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;
using ShipInventory.Helpers;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;

[HarmonyPatch(typeof(ItemManager))]
internal static class ItemManagerPatch
{
    [HarmonyPatch(nameof(ItemManager.SetItems))]
    [HarmonyPostfix]
    private static void SetItemsPatch()
    {
        WebServer.UpdateOverlay();
    }
}
