using System.Collections;
using System.Linq;
using UnityEngine;

namespace com.github.zehsteam.LethalOverlays;

internal static class Utils
{
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

    public static int GetCrewCount()
    {
        if (NetworkUtils.IsServer)
        {
            if (GameNetworkManager.Instance == null)
            {
                return 0;
            }

            return GameNetworkManager.Instance.connectedPlayers;
        }

        if (StartOfRound.Instance == null)
        {
            return 0;
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

    public static int GetDayCount()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.gameStats == null)
        {
            return 0;
        }

        int dayOffset = 0;

        if (!NetworkUtils.IsServer)
        {
            dayOffset = Plugin.ConfigManager.Overlay_DayOffset.Value;
        }

        return StartOfRound.Instance.gameStats.daysSpent + dayOffset;
    }

    public static int GetProfitQuota()
    {
        if (TimeOfDay.Instance == null)
        {
            return 0;
        }

        return TimeOfDay.Instance.profitQuota;
    }

    public static int GetLootTotal()
    {
        return GetShipLootTotal() + GetVehicleLootTotal();
    }

    public static int GetShipLootTotal()
    {
        Transform hangarShipTransform = GetHangarShipTransform();

        if (hangarShipTransform == null)
        {
            return 0;
        }

        GrabbableObject[] grabbableObjects = hangarShipTransform.GetComponentsInChildren<GrabbableObject>();

        return grabbableObjects.Where(IsValidScrapAndNotHeld).Sum(x => x.scrapValue);
    }

    public static int GetVehicleLootTotal()
    {
        VehicleController vehicleController = Object.FindFirstObjectByType<VehicleController>();

        if (vehicleController == null)
        {
            return 0;
        }

        if (!vehicleController.magnetedToShip)
        {
            return 0;
        }

        GrabbableObject[] grabbableObjects = vehicleController.GetComponentsInChildren<GrabbableObject>();

        return grabbableObjects.Where(IsValidScrapAndNotHeld).Sum(x => x.scrapValue);
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

        if (grabbableObject.deactivated)
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
}
