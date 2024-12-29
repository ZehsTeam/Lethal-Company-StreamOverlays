using BepInEx.Configuration;
using com.github.zehsteam.StreamOverlays.Dependencies;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace com.github.zehsteam.StreamOverlays.Helpers;

internal static class ConfigHelper
{
    #region LethalConfig
    public static void SkipAutoGen()
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.SkipAutoGen();
        }
    }

    public static void AddButton(string section, string name, string description, string buttonText, Action callback)
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.AddButton(section, name, description, buttonText, callback);
        }
    }
    #endregion

    public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, bool requiresRestart, string description, AcceptableValueBase acceptableValues = null, Action<T> settingChanged = null, ConfigFile configFile = null)
    {
        configFile ??= Plugin.Config;

        var configEntry = acceptableValues == null
            ? configFile.Bind(section, key, defaultValue, description)
            : configFile.Bind(section, key, defaultValue, new ConfigDescription(description, acceptableValues));

        if (settingChanged != null)
        {
            configEntry.SettingChanged += (sender, e) => settingChanged?.Invoke(configEntry.Value);
        }

        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.AddConfig(configEntry, requiresRestart);
        }

        return configEntry;
    }

    public static Dictionary<ConfigDefinition, string> GetOrphanedConfigEntries(ConfigFile configFile = null)
    {
        configFile ??= Plugin.Config;

        PropertyInfo orphanedEntriesProp = configFile.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
        return (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile, null);
    }

    public static void SetConfigEntryValue<T>(ConfigEntry<T> configEntry, string value)
    {
        // Check if T is int
        if (typeof(T) == typeof(int) && int.TryParse(value, out int parsedInt))
        {
            configEntry.Value = (T)(object)parsedInt;
        }
        // Check if T is float
        else if (typeof(T) == typeof(float) && float.TryParse(value, out float parsedFloat))
        {
            configEntry.Value = (T)(object)parsedFloat;
        }
        // Check if T is double
        else if (typeof(T) == typeof(double) && double.TryParse(value, out double parsedDouble))
        {
            configEntry.Value = (T)(object)parsedDouble;
        }
        // Check if T is bool
        else if (typeof(T) == typeof(bool) && bool.TryParse(value, out bool parsedBool))
        {
            configEntry.Value = (T)(object)parsedBool;
        }
        // Check if T is string (no parsing needed)
        else if (typeof(T) == typeof(string))
        {
            configEntry.Value = (T)(object)value;
        }
        else
        {
            // Optionally handle unsupported types
            throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
        }
    }

    // Credit to Kittenji.
    public static void ClearUnusedEntries(ConfigFile configFile = null)
    {
        configFile ??= Plugin.Config;

        var orphanedEntries = GetOrphanedConfigEntries(configFile);

        if (orphanedEntries == null)
        {
            return;
        }

        orphanedEntries.Clear();
        configFile.Save();
    }
}
