using BepInEx.Bootstrap;
using com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;
using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;
using ShipInventoryUpdated.Objects;
using ShipInventoryUpdated.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using EventType = Unity.Netcode.NetworkListEvent<ShipInventoryUpdated.Objects.ItemData>.EventType;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy;

internal static class ShipInventoryProxy
{
    public const string PLUGIN_GUID = ShipInventoryUpdated.MyPluginInfo.PLUGIN_GUID;
    public static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID);
            return _enabled.Value;
        }
    }

    private static bool? _enabled;

    // This might not be accurate if items from other rounds are removed and added again.
    // ItemData.PERSISTED_THROUGH_ROUNDS isn't always reliable.
    // object is ItemData
    private static readonly List<object> _itemsAddedThisRound = [];

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void PatchAll(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(InventoryPatch));

            Logger.LogInfo("Applied ShipInventory patches.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply ShipInventory patches. {ex}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static int GetLootTotal(bool onlyFromRound = false)
    {
        try
        {
            if (onlyFromRound)
            {
                return _itemsAddedThisRound.Sum(x => x is ItemData itemData ? itemData.SCRAP_VALUE : 0);
            }

            return Inventory.Items.Sum(x => x.SCRAP_VALUE);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to get the total value from ShipInventory. {ex}");
        }

        return 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ResetRoundData()
    {
        Logger.LogInfo($"ShipInventoryProxy: Reset round data", extended: true);

        _itemsAddedThisRound.Clear();
    }

    public static void HandleItemsChanged(NetworkListEvent<ItemData> changeEvent)
    {
        Logger.LogInfo($"[ShipInventoryProxy.HandleItemsChanged] Type: {changeEvent.Type}, Index: {changeEvent.Index}", extended: true);

        switch (changeEvent.Type)
        {
            case EventType.Add:
                HandleItemAdded(changeEvent.Value);
                break;
            case EventType.Insert:
                HandleItemAdded(changeEvent.Value);
                break;

            case EventType.Remove:
                HandleItemRemoved(changeEvent.Value);
                break;
            case EventType.RemoveAt:
                HandleItemRemoved(changeEvent.Value);
                break;
        }

        if (LootManager.CanUpdateLootTotal())
        {
            LootManager.UpdateLootTotal();
            WebServer.UpdateOverlaysData();
        }
    }

    private static void HandleItemAdded(ItemData item)
    {
        Logger.LogInfo($"[ShipInventoryProxy.HandleItemAdded] ID: {item.ID}, SCRAP_VALUE: ${item.SCRAP_VALUE}, PERSISTED_THROUGH_ROUNDS: {item.PERSISTED_THROUGH_ROUNDS}", extended: true);

        if (CanModifyRoundData())
        {
            if (item.PERSISTED_THROUGH_ROUNDS)
            {
                return;
            }

            _itemsAddedThisRound.Add(item);
        }
    }

    private static void HandleItemRemoved(ItemData item)
    {
        Logger.LogInfo($"[ShipInventoryProxy.HandleItemRemoved] ID: {item.ID}, SCRAP_VALUE: ${item.SCRAP_VALUE}, PERSISTED_THROUGH_ROUNDS: {item.PERSISTED_THROUGH_ROUNDS}", extended: true);

        if (CanModifyRoundData())
        {
            if (item.PERSISTED_THROUGH_ROUNDS)
            {
                return;
            }

            _itemsAddedThisRound.Remove(item);
        }
    }

    private static bool CanModifyRoundData()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.inShipPhase)
        {
            return false;
        }

        return true;
    }
}
