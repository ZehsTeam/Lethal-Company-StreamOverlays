using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;
using ShipInventory.Objects;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;

[HarmonyPatch(typeof(ChuteInteract))]
internal static class ChuteInteractPatch
{
    [HarmonyPatch(nameof(ChuteInteract.SpawnItemClientRpc))]
    [HarmonyPostfix]
    private static void SpawnItemClientRpcPatch()
    {
        if (LootManager.CanUpdateLootTotal())
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }
}
