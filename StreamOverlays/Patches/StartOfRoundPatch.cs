using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;
using System;
using System.Reflection;

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
    private static void OnPlayerConnectedClientRpcPatch()
    {
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
        WebServer.UpdateOverlaysData();
    }

    [HarmonyPatch(nameof(StartOfRound.EndOfGame))]
    [HarmonyPostfix]
    private static void EndOfGamePatch(MethodBase __originalMethod, object[] __args)
    {
        // Check if the method has a parameter named "scrapCollected"
        var parameters = __originalMethod.GetParameters();
        var scrapCollectedIndex = Array.FindIndex(parameters, p => p.Name == "scrapCollected");

        int scrapCollected = 0;

        if (scrapCollectedIndex >= 0)
        {
            // If the parameter exists, extract its value from __args
            scrapCollected = (int)__args[scrapCollectedIndex];
        }
        else
        {
            scrapCollected = Utils.GetScrapValueCollectedThisRound();
        }

        DayManager.AddDayData(scrapCollected);
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
        WebServer.UpdateOverlaysData();
    }
}
