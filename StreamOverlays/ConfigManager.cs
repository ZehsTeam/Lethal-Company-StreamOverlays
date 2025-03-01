﻿using BepInEx.Configuration;
using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Server;
using System;

namespace com.github.zehsteam.StreamOverlays;

internal class ConfigManager
{
    // General
    public ConfigEntry<bool> ExtendedLogging { get; private set; }

    // Crew Stat
    public ConfigEntry<string> CrewStat_Label { get; private set; }

    // Moon Stat
    public ConfigEntry<string> MoonStat_Label { get; private set; }
    public ConfigEntry<bool> MoonStat_ShowWeatherIcon { get; private set; }

    // Day Stat
    public ConfigEntry<string> DayStat_Label { get; private set; }

    // Quota Stat
    public ConfigEntry<string> QuotaStat_Label { get; private set; }

    // Loot Stat
    public ConfigEntry<string> LootStat_Label { get; private set; }
    public ConfigEntry<bool> LootStat_OnlyUpdateEndOfDay { get; private set; }

    // Average Per Day Stat
    public ConfigEntry<string> AveragePerDayStat_Label { get; private set; }

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

        // Crew Stat
        CrewStat_Label = ConfigHelper.Bind("Crew Stat", "Label", defaultValue: "Crew: {value}", requiresRestart: false, "The formatting of the Crew stat display text. {value} is the amount of players in the current lobby.");
        CrewStat_Label.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysFormatting();

        // Moon Stat
        MoonStat_Label =           ConfigHelper.Bind("Moon Stat", "Label",           defaultValue: "Moon: {value}", requiresRestart: false, "The formatting of the Moon stat display text. {value} is the name of the current moon.");
        MoonStat_ShowWeatherIcon = ConfigHelper.Bind("Moon Stat", "ShowWeatherIcon", defaultValue: true, requiresRestart: false, "If enabled, will show an icon for the current weather after the moon name.");
        MoonStat_Label.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysFormatting();
        MoonStat_ShowWeatherIcon.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysData();

        // Day Stat
        DayStat_Label = ConfigHelper.Bind("Day Stat", "Label", defaultValue: "Day: {value} ({value2}/{value3})", requiresRestart: false, "The formatting of the Day stat display text. {value} is the day number. {value2} is the day number in the current quota. {value3} is the amount of days in a quota. You can remove {value2} and {value3} if you want to.");
        DayStat_Label.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysFormatting();

        // Quota Stat
        QuotaStat_Label = ConfigHelper.Bind("Quota Stat", "Label", defaultValue: "Quota {value2}: ${value}", requiresRestart: false, "The formatting of the Quota stat display text. {value} is the current profit quota. {value2} is the quota number/index. You can remove {value2} if you want to.");
        QuotaStat_Label.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysFormatting();

        // Loot Stat
        LootStat_Label =              ConfigHelper.Bind("Loot Stat", "Label",              defaultValue: "Ship Loot: ${value}", requiresRestart: false, "The formatting of the Loot stat display text. {value} is the total scrap value on the ship and attached company cruiser.");
        LootStat_OnlyUpdateEndOfDay = ConfigHelper.Bind("Loot Stat", "OnlyUpdateEndOfDay", defaultValue: true,                  requiresRestart: false, "If enabled, the Loot stat will only update when the day ends or if you are in orbit.");
        LootStat_Label.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysFormatting();

        // Average Per Day Stat
        AveragePerDayStat_Label = ConfigHelper.Bind("Average Per Day Stat", "Label", defaultValue: "Avg/Day: ${value}", requiresRestart: false, "The formatting of the Average Per Day stat display text. {value} is the average collected scrap per day.");
        AveragePerDayStat_Label.SettingChanged += (object sender, EventArgs e) => WebServer.UpdateOverlaysFormatting();

        // Server
        Server_AutoStart =     ConfigHelper.Bind("Server", "AutoStart",     defaultValue: true, requiresRestart: false, "If enabled, the server will automatically start when you launch the game.");
        ConfigHelper.AddButton("Server", "Start Server", "Start the server.", "Start", WebServer.Start);
        ConfigHelper.AddButton("Server", "Stop Server", "Stop the server.", "Stop", WebServer.Stop);
        Server_HttpPort =      ConfigHelper.Bind("Server", "HttpPort",      defaultValue: 8080, requiresRestart: false, "The HTTP port for the server.");
        Server_WebSocketPort = ConfigHelper.Bind("Server", "WebSocketPort", defaultValue: 8000, requiresRestart: false, "The WebSocket port for the server.");
    }
}
