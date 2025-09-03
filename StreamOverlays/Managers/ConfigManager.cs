using BepInEx.Configuration;
using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Server;

namespace com.github.zehsteam.StreamOverlays.Managers;

internal static class ConfigManager
{
    public static ConfigFile ConfigFile { get; private set; }

    // General
    public static ConfigEntry<bool> ExtendedLogging { get; private set; }

    // Crew Stat
    public static ConfigEntry<string> CrewStat_Label { get; private set; }

    // Moon Stat
    public static ConfigEntry<string> MoonStat_Label { get; private set; }
    public static ConfigEntry<bool> MoonStat_ShowWeatherIcon { get; private set; }

    // Day Stat
    public static ConfigEntry<string> DayStat_Label { get; private set; }

    // Quota Stat
    public static ConfigEntry<string> QuotaStat_Label { get; private set; }

    // Loot Stat
    public static ConfigEntry<string> LootStat_Label { get; private set; }
    public static ConfigEntry<bool> LootStat_OnlyUpdateEndOfDay { get; private set; }

    // Average Per Day Stat
    public static ConfigEntry<string> AveragePerDayStat_Label { get; private set; }

    // Server
    public static ConfigEntry<bool> Server_AutoStart { get; private set; }
    public static ConfigEntry<int> Server_HttpPort { get; private set; }
    public static ConfigEntry<int> Server_WebSocketPort { get; private set; }

    public static void Initialize(ConfigFile configFile)
    {
        ConfigFile = configFile;
        BindConfigs();
    }

    private static void BindConfigs()
    {
        ConfigHelper.SkipAutoGen();

        // General
        ExtendedLogging = ConfigHelper.Bind("General", "ExtendedLogging", defaultValue: false, "Enable extended logging.");

        // Crew Stat
        CrewStat_Label = ConfigHelper.Bind("Crew Stat", "Label", defaultValue: "Crew: {value}", "The formatting of the Crew stat display text. {value} is the amount of players in the current lobby.");
        CrewStat_Label.SettingChanged += (_, _) => WebServer.UpdateOverlaysFormatting();

        // Moon Stat
        MoonStat_Label =           ConfigHelper.Bind("Moon Stat", "Label",           defaultValue: "Moon: {value}", "The formatting of the Moon stat display text. {value} is the name of the current moon.");
        MoonStat_ShowWeatherIcon = ConfigHelper.Bind("Moon Stat", "ShowWeatherIcon", defaultValue: true,            "If enabled, will show an icon for the current weather after the moon name.");
        MoonStat_Label.SettingChanged += (_, _) => WebServer.UpdateOverlaysFormatting();
        MoonStat_ShowWeatherIcon.SettingChanged += (_, _) => WebServer.UpdateOverlaysData();

        // Day Stat
        DayStat_Label = ConfigHelper.Bind("Day Stat", "Label", defaultValue: "Day: {value} ({value2}/{value3})", "The formatting of the Day stat display text. {value} is the day number. {value2} is the day number in the current quota. {value3} is the amount of days in a quota. You can remove {value2} and {value3} if you want to.");
        DayStat_Label.SettingChanged += (_, _) => WebServer.UpdateOverlaysFormatting();

        // Quota Stat
        QuotaStat_Label = ConfigHelper.Bind("Quota Stat", "Label", defaultValue: "Quota {value2}: ${value}", "The formatting of the Quota stat display text. {value} is the current profit quota. {value2} is the quota number/index. You can remove {value2} if you want to.");
        QuotaStat_Label.SettingChanged += (_, _) => WebServer.UpdateOverlaysFormatting();

        // Loot Stat
        LootStat_Label =              ConfigHelper.Bind("Loot Stat", "Label",              defaultValue: "Ship Loot: ${value}", "The formatting of the Loot stat display text. {value} is the total scrap value on the ship and attached company cruiser.");
        LootStat_OnlyUpdateEndOfDay = ConfigHelper.Bind("Loot Stat", "OnlyUpdateEndOfDay", defaultValue: true,                  "If enabled, the Loot stat will only update when the day ends or if you are in orbit.");
        LootStat_Label.SettingChanged += (_, _) => WebServer.UpdateOverlaysFormatting();

        // Average Per Day Stat
        AveragePerDayStat_Label = ConfigHelper.Bind("Average Per Day Stat", "Label", defaultValue: "Avg/Day: ${value}", "The formatting of the Average Per Day stat display text. {value} is the average collected scrap per day.");
        AveragePerDayStat_Label.SettingChanged += (_, _) => WebServer.UpdateOverlaysFormatting();

        // Server
        Server_AutoStart =     ConfigHelper.Bind("Server", "AutoStart",     defaultValue: true, "If enabled, the server will automatically start when you launch the game.");
        ConfigHelper.AddButton("Server", "Start Server", "Start", "Start the server.", WebServer.Start);
        ConfigHelper.AddButton("Server", "Stop Server", "Stop", "Stop the server.", WebServer.Stop);
        Server_HttpPort =      ConfigHelper.Bind("Server", "HttpPort",      defaultValue: 8080, "The HTTP port for the server.");
        Server_WebSocketPort = ConfigHelper.Bind("Server", "WebSocketPort", defaultValue: 8000, "The WebSocket port for the server.");
    }
}
