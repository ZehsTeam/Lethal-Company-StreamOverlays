using com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy;
using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        DayManager.LoadDayData();
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.OnPlayerConnectedClientRpc))]
    [HarmonyPostfix]
    private static void OnPlayerConnectedClientRpcPatch(ulong clientId)
    {
        if (NetworkUtils.IsLocalClientId(clientId))
        {
            Utils.UpdateScrapPersistedThroughRounds();
        }

        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.SyncShipUnlockablesClientRpc))]
    [HarmonyPostfix]
    private static void SyncShipUnlockablesClientRpcPatch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.OnClientConnect))]
    [HarmonyPostfix]
    private static void OnClientConnectPatch(ulong clientId)
    {
        WebServer.UpdateOverlaysData();
        PluginNetworkManager.OnClientConnected(clientId);
    }

    [HarmonyPatch(nameof(StartOfRound.OnPlayerDC))]
    [HarmonyPostfix]
    private static void OnPlayerDCPatch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    [HarmonyPostfix]
    private static void ChangeLevelClientRpcPatch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    private static void StartGamePatch()
    {
        if (ShipInventoryProxy.Enabled)
        {
            ShipInventoryProxy.ResetRoundData();
        }

        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.EndOfGame))]
    [HarmonyPrefix]
    private static void EndOfGamePatch()
    {
        int value = Utils.GetScrapValueCollectedThisRound();

        Logger.LogInfo($"\n\n\n\nStartOfRoundPatch.EndOfGamePatch() value: {value}\n\n\n", extended: true);

        DayManager.AddDayData(value);
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.ReviveDeadPlayers))]
    [HarmonyPostfix]
    private static void ReviveDeadPlayersPatch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.SetMapScreenInfoToCurrentLevel))]
    [HarmonyPostfix]
    private static void SetMapScreenInfoToCurrentLevelPatch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    private static void ResetShipPatch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.OnLocalDisconnect))]
    [HarmonyPostfix]
    private static void OnLocalDisconnectPatch()
    {
        DayManager.ResetDayData();
        WebServer.UpdateOverlaysData();
    }
}
