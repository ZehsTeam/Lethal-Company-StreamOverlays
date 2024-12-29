using com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy;
using com.github.zehsteam.StreamOverlays.Dependencies.Vanilla;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.github.zehsteam.StreamOverlays;

internal static class LootManager
{
    private static int _shipLootTotal;
    private static int _vehicleLootTotal;
    private static int _shipInventoryLootTotal;

    public static bool CanUpdateLootTotal()
    {
        if (!Plugin.ConfigManager.LootStat_OnlyUpdateEndOfDay.Value)
        {
            return true;
        }

        if (StartOfRound.Instance == null || StartOfRound.Instance.currentLevel == null)
        {
            return false;
        }

        if (!StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap)
        {
            return true;
        }

        return StartOfRound.Instance.inShipPhase;
    }

    public static void UpdateLootTotal()
    {
        _shipLootTotal = GetShipLootTotal();

        if (VehicleControllerProxy.Enabled)
        {
            _vehicleLootTotal = VehicleControllerProxy.GetLootTotal();
        }

        if (ShipInventoryProxy.Enabled)
        {
            _shipInventoryLootTotal = ShipInventoryProxy.GetLootTotal();
        }
    }

    public static int GetLootTotal()
    {
        return _shipLootTotal + _vehicleLootTotal + _shipInventoryLootTotal;
    }

    private static int GetShipLootTotal()
    {
        Transform hangarShipTransform = Utils.GetHangarShipTransform();
        if (hangarShipTransform == null) return 0;

        List<GrabbableObject> grabbableObjects = hangarShipTransform.GetComponentsInChildren<GrabbableObject>().ToList();
        grabbableObjects.AddRange(GetGrabbableObjectsFromShipPlaceableObjects());

        return grabbableObjects.Where(Utils.IsValidScrapAndNotHeld).Sum(x => x.scrapValue);
    }

    private static List<GrabbableObject> GetGrabbableObjectsFromShipPlaceableObjects()
    {
        List<GrabbableObject> grabbableObjects = [];

        foreach (var autoParentToShip in Object.FindObjectsByType<AutoParentToShip>(FindObjectsSortMode.None))
        {
            if (autoParentToShip.transform.parent != null)
            {
                continue;
            }

            grabbableObjects.AddRange(autoParentToShip.GetComponentsInChildren<GrabbableObject>());
        }

        return grabbableObjects;
    }
}
