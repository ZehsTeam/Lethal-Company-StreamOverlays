using BepInEx.Configuration;
using System;

namespace com.github.zehsteam.LethalOverlays;

internal class ConfigManager
{
    // General
    public ConfigEntry<bool> ExtendedLogging { get; private set; }

    // Overlay
    public ConfigEntry<int> Overlay_DayOffset { get; private set; }
    public ConfigEntry<bool> Overlay_ShowWeatherIcon { get; private set; }

    // Networking
    public ConfigEntry<bool> Networking_Enabled { get; private set; }
    public ConfigEntry<string> Networking_IP { get; private set; }
    public ConfigEntry<int> Networking_Port { get; private set; }
    public ConfigEntry<bool> Networking_AutoReconnect { get; private set; }
    public ConfigEntry<float> Networking_ReconnectDelay { get; private set; }

    public ConfigManager()
    {
        BindConfigs();
        ConfigHelper.ClearUnusedEntries();
    }

    private void BindConfigs()
    {
        ConfigHelper.SkipAutoGen();

        // General
        ExtendedLogging = ConfigHelper.Bind("General", "ExtendedLogging", defaultValue: false, requiresRestart: false, "Enable extended logging.");

        // Overlay
        Overlay_DayOffset =       ConfigHelper.Bind("Overlay", "DayOffset",       defaultValue: 0,    requiresRestart: false, "The day offset. If you are playing multiplayer and you are not the host, the day count can be desynced.");
        Overlay_ShowWeatherIcon = ConfigHelper.Bind("Overlay", "ShowWeatherIcon", defaultValue: true, requiresRestart: false, "If enabled, will show the current weather as an icon after the moon name.");
        ConfigHelper.AddButton("Overlay", "Refresh Overlay", "Refresh the overlay.", "Refresh", WebSocketClient.UpdateOverlay);

        Overlay_DayOffset.SettingChanged += (object sender, EventArgs e) => WebSocketClient.UpdateOverlay();
        Overlay_ShowWeatherIcon.SettingChanged += (object sender, EventArgs e) => WebSocketClient.UpdateOverlay();

        // Networking
        Networking_Enabled =        ConfigHelper.Bind("Networking", "Enabled",        defaultValue: true,        requiresRestart: false, "Enable networking and connection to the server.");
        Networking_IP =             ConfigHelper.Bind("Networking", "IP",             defaultValue: "127.0.0.1", requiresRestart: false, "The ip address for the server.");
        Networking_Port =           ConfigHelper.Bind("Networking", "Port",           defaultValue: 8080,        requiresRestart: false, "The port for the server.");
        Networking_AutoReconnect =  ConfigHelper.Bind("Networking", "AutoReconnect",  defaultValue: true,        requiresRestart: false, "If enabled, will try to automatically reconnect to the server.");
        Networking_ReconnectDelay = ConfigHelper.Bind("Networking", "ReconnectDelay", defaultValue: 5f,          requiresRestart: false, "The delay in seconds before trying to reconnect to the server.", new AcceptableValueRange<float>(1.0f, 60.0f));
        ConfigHelper.AddButton("Networking", "Refresh Connection", "Refresh the connection to the server.", "Refresh", WebSocketClient.Reconnect);
        ConfigHelper.AddButton("Networking", "Disconnect Connection", "Disconnect the connection to the server.", "Disconnect", WebSocketClient.CloseConnection);
        
        Networking_Enabled.SettingChanged += Networking_Enabled_SettingChanged;
        Networking_IP.SettingChanged += (object sender, EventArgs e) => WebSocketClient.Reconnect();
        Networking_Port.SettingChanged += (object sender, EventArgs e) => WebSocketClient.Reconnect();
    }

    private void Networking_Enabled_SettingChanged(object sender, EventArgs e)
    {
        if (Networking_Enabled.Value)
        {
            WebSocketClient.Reconnect();
        }
        else
        {
            WebSocketClient.CloseConnection();
        }
    }
}
