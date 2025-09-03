using BepInEx.Configuration;

namespace com.github.zehsteam.StreamOverlays.Extensions;

internal static class ConfigFileExtensions
{
    public static ConfigEntry<T> Bind<T>(this ConfigFile configFile, string section, string key, T defaultValue, string description, AcceptableValueBase acceptableValues)
    {
        if (acceptableValues == null)
        {
            return configFile.Bind(section, key, defaultValue, description);
        }

        return configFile.Bind(section, key, defaultValue, new ConfigDescription(description, acceptableValues));
    }
}
