using BepInEx.Configuration;
using com.github.zehsteam.StreamOverlays.Dependencies;
using com.github.zehsteam.StreamOverlays.Extensions;
using com.github.zehsteam.StreamOverlays.Managers;
using System;

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

    public static void AddButton(string section, string name, string buttonText, string description, Action callback)
    {
        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.AddButton(section, name, buttonText, description, callback);
        }
    }
    #endregion

    public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description, bool requiresRestart = false, AcceptableValueBase acceptableValues = null, Action<T> settingChanged = null, ConfigFile configFile = null)
    {
        configFile ??= ConfigManager.ConfigFile;

        var configEntry = configFile.Bind(section, key, defaultValue, description, acceptableValues);

        if (settingChanged != null)
        {
            configEntry.SettingChanged += (_, _) => settingChanged?.Invoke(configEntry.Value);
        }

        if (LethalConfigProxy.Enabled)
        {
            LethalConfigProxy.AddConfig(configEntry, requiresRestart);
        }

        return configEntry;
    }
}
