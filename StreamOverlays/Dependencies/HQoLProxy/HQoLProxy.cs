using BepInEx.Bootstrap;
using com.github.zehsteam.StreamOverlays.Dependencies.HQoLProxy.Patches;
using HarmonyLib;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace com.github.zehsteam.StreamOverlays.Dependencies.HQoLProxy;

internal static class HQoLProxy
{
    public const string PLUGIN_72_GUID = "OreoM.HQoL.72";
    public const string PLUGIN_72_ASSEMBLY_NAME = "OreoM.HQoL.72";

    public const string PLUGIN_73_GUID = "OreoM.HQoL.73";
    public const string PLUGIN_73_ASSEMBLY_NAME = "OreoM.HQoL.73";

    public static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey(PLUGIN_72_GUID) || Chainloader.PluginInfos.ContainsKey(PLUGIN_73_GUID);
            return _enabled.Value;
        }
    }

    private static bool? _enabled;

    private static int storageValueAtStartOfRound = 0;

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void PatchAll(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(StartOfRoundPatch));

            Logger.LogInfo("Applied ShipInventory patches.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply ShipInventory patches. {ex}");
        }
    }

    private static int GetTotalStorageValue()
    {
        var asm72 = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == PLUGIN_72_ASSEMBLY_NAME);
        var asm73 = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == PLUGIN_73_ASSEMBLY_NAME);

        var asm = asm73 ?? asm72 ?? throw new Exception("HQoL assembly not found.");

        var networkType = asm.GetType("HQoL.Network.HQoLNetwork", throwOnError: false)
            ?? throw new Exception("HQoLNetwork type not found.");

        var networkInstanceProperty = AccessTools.Property(networkType, "Instance")
            ?? throw new Exception("HQoLNetwork Instance property not found.");

        var networkInstance = networkInstanceProperty.GetValue(null)
            ?? throw new Exception("HQoLNetwork Instance is null.");

        // NetworkVariable<int>
        var totalStorageValueField = AccessTools.Field(networkType, "totalStorageValue")
            ?? throw new Exception("HQoLNetwork totalStorageValue field not found.");

        var totalStorageValueNetworkVariable = totalStorageValueField.GetValue(networkInstance)
            ?? throw new Exception("HQoLNetwork totalStorageValue is null.");

        var valueProperty = totalStorageValueNetworkVariable.GetType().GetProperty("Value")
            ?? throw new Exception("HQoLNetwork totalStorageValue Value property not found.");

        return (int)valueProperty.GetValue(totalStorageValueNetworkVariable);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static int GetLootTotal(bool onlyFromRound = false)
    {
        var lootTotal = 0;

        try
        {
            lootTotal = GetTotalStorageValue();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to get the storage value from HQoL. {ex}");
        }

        if (onlyFromRound)
        {
            lootTotal -= storageValueAtStartOfRound;
            lootTotal = Math.Max(lootTotal, 0);
        }

        return lootTotal;
    }

    public static void UpdateStorageValueAtStartOfRound()
    {
        try
        {
           storageValueAtStartOfRound = GetTotalStorageValue();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to get the storage value from HQoL. {ex}");
        }
    }
}
