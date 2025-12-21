using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace com.github.zehsteam.StreamOverlays.Dependencies;

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

    private static int GetTotalStorageValueByAssembly(Assembly assembly)
    {
        // internal class
        var networkType = assembly.GetType("HQoL.Network.HQoLNetwork", throwOnError: false)
            ?? throw new Exception("HQoLNetwork type not found.");

        var networkInstanceProperty = AccessTools.Property(networkType, "Instance")
            ?? throw new Exception("HQoLNetwork Instance property not found.");

        var networkInstance = networkInstanceProperty.GetValue(null)
            ?? throw new HQoLNotInitializedException($"HQoL is not initialized. Assembly={assembly.FullName}");

        // NetworkVariable<int>
        var totalStorageValueField = AccessTools.Field(networkType, "totalStorageValue")
            ?? throw new Exception("HQoLNetwork totalStorageValue field not found.");

        var totalStorageValueNetworkVariable = totalStorageValueField.GetValue(networkInstance)
            ?? throw new Exception("HQoLNetwork totalStorageValue is null.");

        var valueProperty = totalStorageValueNetworkVariable.GetType().GetProperty("Value")
            ?? throw new Exception("HQoLNetwork totalStorageValue Value property not found.");

        var value = valueProperty.GetValue(totalStorageValueNetworkVariable)
            ?? throw new Exception("HQoLNetwork totalStorageValue Value is null.");

        if (value is int intValue)
        {
            return intValue;
        }

        throw new Exception("HQoLNetwork totalStorageValue Value is not an integer.");
    }

    private static int GetTotalStorageValue()
    {
        var assmblies = AppDomain.CurrentDomain.GetAssemblies();
        var asm73 = assmblies
            .FirstOrDefault(a => a.GetName().Name == PLUGIN_73_ASSEMBLY_NAME);
        var asm72 = assmblies
            .FirstOrDefault(a => a.GetName().Name == PLUGIN_72_ASSEMBLY_NAME);
        if (asm73 == null && asm72 == null)
        {
            throw new Exception("HQoL assembly not found.");
        }

        if (asm73 != null)
        {
            try
            {
                return GetTotalStorageValueByAssembly(asm73);
            }
            catch (HQoLNotInitializedException)
            {
                // ignore and try the other version

                // check if other version exists
                if (asm72 == null)
                {
                    // no other version to try
                    throw;
                }
            }
        }

        // try backport version
        return GetTotalStorageValueByAssembly(asm72);
    }

    public static int GetLootTotal()
    {
        try
        {
            return GetTotalStorageValue();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to get the storage value from HQoL. {ex}");
        }

        return 0;
    }
}

internal class HQoLNotInitializedException : Exception
{
    public HQoLNotInitializedException(string message) : base(message)
    {
    }
}
