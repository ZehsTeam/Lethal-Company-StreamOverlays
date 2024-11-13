using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.OnPlayerConnectedClientRpc))]
    [HarmonyPostfix]
    private static void OnPlayerConnectedClientRpcPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.SyncShipUnlockablesClientRpc))]
    [HarmonyPostfix]
    private static void SyncShipUnlockablesClientRpcPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.OnClientConnect))]
    [HarmonyPostfix]
    private static void OnClientConnectPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.OnPlayerDC))]
    [HarmonyPostfix]
    private static void OnPlayerDCPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    [HarmonyPostfix]
    private static void ChangeLevelClientRpcPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    private static void StartGamePatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.EndOfGameClientRpc))]
    [HarmonyPostfix]
    private static void EndOfGameClientRpcPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.ReviveDeadPlayers))]
    [HarmonyPostfix]
    private static void ReviveDeadPlayersPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.SetMapScreenInfoToCurrentLevel))]
    [HarmonyPostfix]
    private static void SetMapScreenInfoToCurrentLevelPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    private static void ResetShipPatch()
    {
        WebSocketClient.UpdateOverlay();
    }

    [HarmonyPatch(nameof(StartOfRound.OnLocalDisconnect))]
    [HarmonyPostfix]
    private static void OnLocalDisconnectPatch()
    {
        WebSocketClient.UpdateOverlay();
    }
}
