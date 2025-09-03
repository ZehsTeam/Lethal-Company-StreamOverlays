namespace com.github.zehsteam.StreamOverlays.Helpers;

internal static class GameSaveFileHelper
{
    private static string GetCurrentSaveFilePath()
    {
        if (GameNetworkManager.Instance == null)
        {
            Logger.LogWarning("GameSaveFileHelper: GetCurrentSaveFilePath() GameNetworkManager instance is null. Returning an empty string.");
            return string.Empty;
        }

        return GameNetworkManager.Instance.currentSaveFileName;
    }

    public static bool KeyExists(string key)
    {
        if (!NetworkUtils.IsServer)
            return false;

        return ES3.KeyExists(key, GetCurrentSaveFilePath());
    }

    public static T Load<T>(string key, T defaultValue = default)
    {
        if (!NetworkUtils.IsServer)
            return defaultValue;

        if (TryLoad(key, out T value))
        {
            return value;
        }

        return defaultValue;
    }

    public static bool TryLoad<T>(string key, out T value)
    {
        if (!NetworkUtils.IsServer)
        {
            value = default;
            return false;
        }

        value = ES3.Load<T>(key, GetCurrentSaveFilePath());
        return value != null;
    }

    public static bool Save<T>(string key, T value)
    {
        if (!NetworkUtils.IsServer)
            return false;

        ES3.Save(key, value, GetCurrentSaveFilePath());
        return true;
    }
}
