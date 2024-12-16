using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(VehicleController))]
internal static class VehicleControllerPatch
{
    [HarmonyPatch(nameof(VehicleController.CollectItemsInTruck))]
    [HarmonyPostfix]
    private static void CollectItemsInTruckPatch()
    {
        WebServer.UpdateOverlaysData();
    }
}
