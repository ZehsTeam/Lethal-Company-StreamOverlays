using com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy;
using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class StartOfRound_Patches
{
    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void Start_Patch()
    {
        DayManager.LoadDayData();
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.OnPlayerConnectedClientRpc))]
    [HarmonyPostfix]
    private static void OnPlayerConnectedClientRpc_Patch(ulong clientId)
    {
        if (NetworkUtils.IsLocalClientId(clientId))
        {
            Utils.UpdateScrapPersistedThroughRounds();
        }

        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.SyncShipUnlockablesClientRpc))]
    [HarmonyPostfix]
    private static void SyncShipUnlockablesClientRpc_Patch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.OnClientConnect))]
    [HarmonyPostfix]
    private static void OnClientConnect_Patch(ulong clientId)
    {
        WebServer.UpdateOverlaysData();
        PluginNetworkManager.OnClientConnected(clientId);
    }

    [HarmonyPatch(nameof(StartOfRound.OnPlayerDC))]
    [HarmonyPostfix]
    private static void OnPlayerDC_Patch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    [HarmonyPostfix]
    private static void ChangeLevelClientRpc_Patch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    private static void StartGame_Patch()
    {
        if (ShipInventoryProxy.Enabled)
        {
            ShipInventoryProxy.ResetRoundData();
        }

        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.EndOfGame))]
    [HarmonyPrefix]
    private static void EndOfGame_Patch()
    {
        DayManager.AddDayData(Utils.GetScrapValueCollectedThisRound());
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.ReviveDeadPlayers))]
    [HarmonyPostfix]
    private static void ReviveDeadPlayers_Patch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.SetMapScreenInfoToCurrentLevel))]
    [HarmonyPostfix]
    private static void SetMapScreenInfoToCurrentLevel_Patch()
    {
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    private static void ResetShip_Patch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.OnLocalDisconnect))]
    [HarmonyPostfix]
    private static void OnLocalDisconnect_Patch()
    {
        DayManager.ResetDayData();
        WebServer.UpdateOverlaysData();
    }
}
