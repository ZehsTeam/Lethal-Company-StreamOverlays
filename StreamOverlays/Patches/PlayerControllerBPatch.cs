using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerControllerBPatch
{
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    [HarmonyPostfix]
    private static void ConnectClientToPlayerObjectPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(PlayerControllerB.GrabObjectClientRpc))]
    [HarmonyPostfix]
    private static void GrabObjectClientRpcPatch(NetworkObjectReference grabbedObject)
    {
        #pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
        if (!grabbedObject.TryGet(out NetworkObject networkObject))
        {
            return;
        }
        #pragma warning restore Harmony003 // Harmony non-ref patch parameters modified

        if (!networkObject.TryGetComponent(out GrabbableObject grabbableObject))
        {
            return;
        }

        if (grabbableObject.isInShipRoom || grabbableObject.isInElevator)
        {
            WebSocketClient.UpdateOverlay(); // Update Loot
        }
    }

    [HarmonyPatch(nameof(PlayerControllerB.ThrowObjectClientRpc))]
    [HarmonyPostfix]
    private static void ThrowObjectClientRpcPatch(bool droppedInElevator, bool droppedInShipRoom)
    {
        if (droppedInShipRoom || droppedInElevator)
        {
            WebSocketClient.UpdateOverlay(); // Update Loot
        }
    }
}
