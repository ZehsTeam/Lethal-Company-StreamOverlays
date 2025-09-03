using BepInEx.Bootstrap;
using com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;
using HarmonyLib;
using ShipInventory.Items;
using System;
using System.Runtime.CompilerServices;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy;

internal static class ShipInventoryProxy
{
    public const string PLUGIN_GUID = LCMPluginInfo.PLUGIN_GUID;
    public static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID);
            return _enabled.Value;
        }
    }

    private static bool? _enabled;

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void PatchAll(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(ItemManagerPatch));
            harmony.PatchAll(typeof(ChuteInteractPatch));

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
            return ItemManager.GetTotalValue(onlyScraps: true, onlyFromRound);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to get the total value from ShipInventory. {ex}");
        }

        return 0;
    }
}
