extern alias HQoL72;
extern alias HQoL73;

using HQoL72Network = HQoL72::HQoL.Network;
using HQoL73Network = HQoL73::HQoL.Network;

using BepInEx.Bootstrap;
using System;
using System.Runtime.CompilerServices;

namespace com.github.zehsteam.StreamOverlays.Dependencies;

internal static class HQoLProxy
{
    // Backport version for Lethal Company v72 and below
    public const string PLUGIN_72_GUID = "OreoM.HQoL.72";

    // Current version for Lethal Company v73 and above
    public const string PLUGIN_73_GUID = "OreoM.HQoL.73";

    public static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey(PLUGIN_72_GUID) || Chainloader.PluginInfos.ContainsKey(PLUGIN_73_GUID);
            return _enabled.Value;
        }
    }

    private static bool? _enabled;

    private static int GetTotalStorageValue()
    {
        var hqol72NetworkInstance = HQoL72Network.HQoLNetwork.Instance;
        if (hqol72NetworkInstance != null)
        {
            return hqol72NetworkInstance.totalStorageValue.Value;
        }

        var hqol73NetworkInstance = HQoL73Network.HQoLNetwork.Instance;
        if (hqol73NetworkInstance != null)
        {
            return hqol73NetworkInstance.totalStorageValue.Value;
        }

        throw new Exception("HQoL is not initialized.");
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
