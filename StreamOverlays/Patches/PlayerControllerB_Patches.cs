using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerControllerB_Patches
{
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    [HarmonyPostfix]
    private static void ConnectClientToPlayerObject_Patch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(PlayerControllerB.GrabObjectClientRpc))]
    [HarmonyPostfix]
    private static void GrabObjectClientRpc_Patch(NetworkObjectReference grabbedObject)
    {
        if (!LootManager.CanUpdateLootTotal())
        {
            return;
        }

        #pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
        if (!grabbedObject.TryGet(out NetworkObject networkObject))
        #pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
        {
            return;
        }

        if (!networkObject.TryGetComponent(out GrabbableObject grabbableObject))
        {
            return;
        }

        if (grabbableObject.isInShipRoom || grabbableObject.isInElevator)
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }

    [HarmonyPatch(nameof(PlayerControllerB.ThrowObjectClientRpc))]
    [HarmonyPostfix]
    private static void ThrowObjectClientRpc_Patch(bool droppedInElevator, bool droppedInShipRoom)
    {
        if (!LootManager.CanUpdateLootTotal())
        {
            return;
        }

        if (droppedInShipRoom || droppedInElevator)
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }

    [HarmonyPatch(nameof(PlayerControllerB.PlaceGrabbableObject))]
    [HarmonyPostfix]
    private static void PlaceGrabbableObject_Patch(GrabbableObject placeObject)
    {
        if (!LootManager.CanUpdateLootTotal())
        {
            return;
        }

        if (placeObject == null)
        {
            return;
        }

        if (placeObject.isInShipRoom || placeObject.isInElevator)
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }
}
