using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Dependencies.Vanilla.Patches;

[HarmonyPatch(typeof(VehicleController))]
internal static class VehicleController_Patches
{
    [HarmonyPatch(nameof(VehicleController.CollectItemsInTruck))]
    [HarmonyPostfix]
    private static void CollectItemsInTruck_Patch()
    {
        if (LootManager.CanUpdateLootTotal())
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }
}
