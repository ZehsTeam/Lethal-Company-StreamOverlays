using BepInEx.Configuration;

namespace com.github.zehsteam.LethalOverlays;

internal class ConfigManager
{
    // General
    public ConfigEntry<bool> ExtendedLogging { get; private set; }

    // Overlay
    public ConfigEntry<int> Overlay_DayOffset { get; private set; }

    // Networking
    public ConfigEntry<int> Networking_Port { get; private set; }
    public ConfigEntry<bool> Networking_AutoReconnect { get; private set; }
    public ConfigEntry<float> Networking_ReconnectDelay { get; private set; }

    public ConfigManager()
    {
        BindConfigs();
    }

    private void BindConfigs()
    {
        ConfigHelper.SkipAutoGen();

        // General
        ExtendedLogging = ConfigHelper.Bind("General", "ExtendedLogging", defaultValue: false, requiresRestart: false, "Enable extended logging.");

        // Overlay
        Overlay_DayOffset = ConfigHelper.Bind("Overlay", "DayOffset", defaultValue: 0, requiresRestart: false, "The day offset. If you are playing multiplayer and you are not the host, the day count can be desynced. (Only works when you are not the host)");

        // Networking
        Networking_Port =           ConfigHelper.Bind("Networking", "Port",           defaultValue: 8080, requiresRestart: false, "The port for the Node.js server.");
        Networking_AutoReconnect =  ConfigHelper.Bind("Networking", "AutoReconnect",  defaultValue: true, requiresRestart: false, "If enabled, will try to automatically reconnect to the Node.js server.");
        Networking_ReconnectDelay = ConfigHelper.Bind("Networking", "ReconnectDelay", defaultValue: 5f,   requiresRestart: false, "The delay in seconds before trying to reconnect to the Node.js server.", new AcceptableValueRange<float>(1.0f, 60.0f));
        ConfigHelper.AddButton("Networking", "Refresh Connection", "Refresh the connection to the Node.js server.", "Refresh", WebSocketClient.Reconnect);
        ConfigHelper.AddButton("Networking", "Disconnect Connection", "Disconnect the connection to the Node.js server.", "Disconnect", WebSocketClient.CloseConnection);
    }
}
