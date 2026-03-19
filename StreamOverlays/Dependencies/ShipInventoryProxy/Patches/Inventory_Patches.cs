using HarmonyLib;
using ShipInventoryUpdated.Scripts;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;

[HarmonyPatch(typeof(Inventory))]
internal static class Inventory_Patches
{
    [HarmonyPatch(nameof(Inventory.OnNetworkSpawn))]
    [HarmonyPostfix]
    private static void OnNetworkSpawn_Patch(Inventory __instance)
    {
        __instance._storedItems.OnListChanged += ShipInventoryProxy.HandleItemsChanged;
    }

    [HarmonyPatch(nameof(Inventory.OnNetworkDespawn))]
    [HarmonyPrefix]
    private static void OnNetworkDespawn_Patch(Inventory __instance)
    {
        __instance._storedItems.OnListChanged -= ShipInventoryProxy.HandleItemsChanged;
    }
}
