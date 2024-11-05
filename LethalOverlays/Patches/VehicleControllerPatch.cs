using HarmonyLib;

namespace com.github.zehsteam.LethalOverlays.Patches;

[HarmonyPatch(typeof(VehicleController))]
internal static class VehicleControllerPatch
{
    [HarmonyPatch(nameof(VehicleController.CollectItemsInTruck))]
    [HarmonyPostfix]
    private static void CollectItemsInTruckPatch()
    {
        WebSocketClient.UpdateOverlay();
    }
}
