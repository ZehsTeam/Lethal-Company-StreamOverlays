using HarmonyLib;
using ShipInventoryUpdated.Scripts;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;

[HarmonyPatch(typeof(Inventory))]
internal static class InventoryPatch
{
    [HarmonyPatch(nameof(Inventory.OnNetworkSpawn))]
    [HarmonyPostfix]
    private static void OnNetworkSpawnPatch(Inventory __instance)
    {
        __instance._storedItems.OnListChanged += ShipInventoryProxy.HandleItemsChanged;
    }

    [HarmonyPatch(nameof(Inventory.OnNetworkDespawn))]
    [HarmonyPrefix]
    private static void OnNetworkDespawnPatch(Inventory __instance)
    {
        __instance._storedItems.OnListChanged -= ShipInventoryProxy.HandleItemsChanged;
    }
}
