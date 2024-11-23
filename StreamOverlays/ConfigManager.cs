using BepInEx.Configuration;
using System;

namespace com.github.zehsteam.StreamOverlays;

internal class ConfigManager
{
    // General
    public ConfigEntry<bool> ExtendedLogging { get; private set; }

    // Overlay
    public ConfigEntry<int> Overlay_DayOffset { get; private set; }
    public ConfigEntry<bool> Overlay_ShowWeatherIcon { get; private set; }

    // Server
    public ConfigEntry<bool> Server_AutoStart { get; private set; }
    public ConfigEntry<int> Server_Port { get; private set; }

    // Client
    public ConfigEntry<bool> Client_Enabled { get; private set; }
    public ConfigEntry<string> Client_ServerIP { get; private set; }
    public ConfigEntry<int> Client_ServerPort { get; private set; }
    public ConfigEntry<bool> Client_AutoReconnect { get; private set; }
    public ConfigEntry<float> Client_ReconnectDelay { get; private set; }

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

        // Server
        ConfigHelper.AddButton("Server", "Start Server", "Start the server.", "Start", ServerHelper.Start);
        //ConfigHelper.AddButton("Server", "Close Server", "Close the server.", "Close", ServerHelper.Stop);
        Server_AutoStart = ConfigHelper.Bind("Server", "AutoStart", defaultValue: false, requiresRestart: false, "If enabled, the server will automatically start when you launch the game.");
        Server_Port =      ConfigHelper.Bind("Server", "Port",      defaultValue: 8080,  requiresRestart: false, "The port for the server.");
        
        // Client
        Client_Enabled =        ConfigHelper.Bind("Client", "Enabled",        defaultValue: false,       requiresRestart: false, "Enable networking and connection to the server.");
        ConfigHelper.AddButton("Client", "Connect to server", "Connect to the server.", "Connect", WebSocketClient.Reconnect);
        ConfigHelper.AddButton("Client", "Disconnect from server", "Disconnect from the server.", "Disconnect", WebSocketClient.CloseConnection);
        Client_AutoReconnect =  ConfigHelper.Bind("Client", "AutoReconnect",  defaultValue: true,        requiresRestart: false, "If enabled, will try to automatically reconnect to the server.");
        Client_ReconnectDelay = ConfigHelper.Bind("Client", "ReconnectDelay", defaultValue: 5f,          requiresRestart: false, "The delay in seconds before trying to reconnect to the server.", new AcceptableValueRange<float>(1.0f, 60.0f));
        Client_ServerIP =       ConfigHelper.Bind("Client", "ServerIP",       defaultValue: "127.0.0.1", requiresRestart: false, "The ip address for the server.");
        Client_ServerPort =     ConfigHelper.Bind("Client", "ServerPort",     defaultValue: 8080,        requiresRestart: false, "The port for the server.");
        
        Client_Enabled.SettingChanged += Client_Enabled_SettingChanged;
        Client_ServerIP.SettingChanged += (object sender, EventArgs e) => WebSocketClient.Reconnect();
        Client_ServerPort.SettingChanged += (object sender, EventArgs e) => WebSocketClient.Reconnect();
    }

    private void Client_Enabled_SettingChanged(object sender, EventArgs e)
    {
        if (Client_Enabled.Value)
        {
            WebSocketClient.Reconnect();
        }
        else
        {
            WebSocketClient.CloseConnection();
        }
    }
}
