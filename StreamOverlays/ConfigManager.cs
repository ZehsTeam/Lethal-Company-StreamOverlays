using BepInEx.Configuration;
using com.github.zehsteam.StreamOverlays.Server;
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
    public ConfigEntry<int> Server_HttpPort { get; private set; }
    public ConfigEntry<int> Server_WebSocketPort { get; private set; }

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
        ConfigHelper.AddButton("Overlay", "Refresh Overlay", "Refresh the overlay.", "Refresh", WebServer.UpdateOverlay);

        Overlay_DayOffset.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlay();
        Overlay_ShowWeatherIcon.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlay();

        // Server
        Server_AutoStart =     ConfigHelper.Bind("Server", "AutoStart",     defaultValue: true, requiresRestart: false, "If enabled, the server will automatically start when you launch the game.");
        ConfigHelper.AddButton("Server", "Start Server", "Start the server.", "Start", WebServer.Start);
        ConfigHelper.AddButton("Server", "Stop Server", "Stop the server.", "Stop", WebServer.Stop);
        Server_HttpPort =      ConfigHelper.Bind("Server", "HttpPort",      defaultValue: 8080,  requiresRestart: false, "The HTTP port for the server.");
        Server_WebSocketPort = ConfigHelper.Bind("Server", "WebSocketPort", defaultValue: 8000,  requiresRestart: false, "The WebSocket port for the server.");
    }
}
