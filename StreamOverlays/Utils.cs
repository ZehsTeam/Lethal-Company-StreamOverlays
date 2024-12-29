using BepInEx;
using BepInEx.Configuration;
using com.github.zehsteam.StreamOverlays.Helpers;
using System.Collections;
using System.IO;
using UnityEngine;

namespace com.github.zehsteam.StreamOverlays;

internal static class Utils
{
    public static string GetEnumName<T>(T e) where T : System.Enum
    {
        return System.Enum.GetName(typeof(T), e) ?? string.Empty;
    }

    public static string GetPluginDirectoryPath()
    {
        return Path.GetDirectoryName(Plugin.Instance.Info.Location);
    }

    public static ConfigFile CreateConfigFile(string path, string name = null, bool saveOnInit = false)
    {
        BepInPlugin metadata = MetadataHelper.GetMetadata(Plugin.Instance);
        name ??= metadata.GUID;
        name += ".cfg";
        return new ConfigFile(Path.Combine(path, name), saveOnInit, metadata);
    }

    public static ConfigFile CreateLocalConfigFile(string name = null, bool saveOnInit = false)
    {
        BepInPlugin metadata = MetadataHelper.GetMetadata(Plugin.Instance);
        name ??= $"{metadata.GUID}-{name}";
        return CreateConfigFile(Paths.ConfigPath, name, saveOnInit);
    }

    public static ConfigFile CreateGlobalConfigFile(string name = null, bool saveOnInit = false)
    {
        BepInPlugin metadata = MetadataHelper.GetMetadata(Plugin.Instance);
        string path = Path.Combine(Application.persistentDataPath, metadata.Name);
        name ??= "global";
        return CreateConfigFile(path, name, saveOnInit);
    }

    public static Transform GetHangarShipTransform()
    {
        if (StartOfRound.Instance == null)
        {
            return null;
        }

        return StartOfRound.Instance.elevatorTransform;
    }

    public static bool CanShowOverlay()
    {
        if (GameNetworkManager.Instance == null)
        {
            return false;
        }

        if (GameNetworkManager.Instance.isDisconnecting)
        {
            return false;
        }

        if (!PlayerUtils.IsLocalPlayerSpawned())
        {
            return false;
        }

        return true;
    }

    public static int GetPlayerCount()
    {
        if (NetworkUtils.IsServer)
        {
            if (GameNetworkManager.Instance == null)
            {
                return 1;
            }

            return GameNetworkManager.Instance.connectedPlayers;
        }

        if (StartOfRound.Instance == null)
        {
            return 1;
        }

        return StartOfRound.Instance.connectedPlayersAmount + 1;
    }

    public static string GetCurrentPlanetName()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.currentLevel == null)
        {
            return string.Empty;
        }

        return StartOfRound.Instance.currentLevel.PlanetName;
    }

    public static LevelWeatherType GetCurrentPlanetWeather()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.currentLevel == null)
        {
            return LevelWeatherType.None;
        }

        return StartOfRound.Instance.currentLevel.currentWeather;
    }

    public static int GetDayInQuota()
    {
        if (TimeOfDay.Instance == null)
        {
            return 1;
        }

        int daysUntilDeadline = Mathf.Max(TimeOfDay.Instance.daysUntilDeadline, 0);
        int daysInQuota = 3;

        return (daysInQuota - daysUntilDeadline) + 1;
    }

    public static int GetProfitQuota()
    {
        if (TimeOfDay.Instance == null)
        {
            return 0;
        }

        return TimeOfDay.Instance.profitQuota;
    }

    public static int GetQuotaIndex()
    {
        if (TimeOfDay.Instance == null)
        {
            return 1;
        }

        return TimeOfDay.Instance.timesFulfilledQuota + 1;
    }

    public static bool IsValidScrapAndNotHeld(GrabbableObject grabbableObject)
    {
        if (!IsValidScrap(grabbableObject))
        {
            return false;
        }

        if (grabbableObject.isHeld)
        {
            return false;
        }

        return true;
    }

    public static bool IsValidScrap(GrabbableObject grabbableObject)
    {
        if (grabbableObject == null || grabbableObject.itemProperties == null)
        {
            return false;
        }

        if (GrabbableObjectHelper.IsDeactivated(grabbableObject))
        {
            return false;
        }

        return grabbableObject.itemProperties.isScrap;
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (Plugin.Instance != null)
        {
            return Plugin.Instance.StartCoroutine(routine);
        }

        if (GameNetworkManager.Instance != null)
        {
            return GameNetworkManager.Instance.StartCoroutine(routine);
        }

        Plugin.Logger.LogError("Failed to start coroutine. " + routine);

        return null;
    }

    public static int GetScrapValueCollectedThisRound()
    {
        if (StartOfRound.Instance == null)
        {
            return 0;
        }

        GrabbableObject[] grabbableObjects = Object.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None);

        int totalValue = 0;

        foreach (var grabbableObject in grabbableObjects)
        {
            if (!grabbableObject.itemProperties.isScrap)
            {
                continue;
            }

            if (grabbableObject.scrapPersistedThroughRounds)
            {
                continue;
            }

            if (GrabbableObjectHelper.IsDeactivated(grabbableObject) || grabbableObject.itemUsedUp)
            {
                continue;
            }

            if (grabbableObject.isInShipRoom || grabbableObject.isInElevator)
            {
                totalValue += grabbableObject.scrapValue;
            }
        }

        return totalValue;
    }
}
