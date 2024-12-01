using System.Reflection;

namespace com.github.zehsteam.StreamOverlays;

internal static class SaveSystem
{
    public static bool KeyExists(string key)
    {
        return ES3.KeyExists($"{GetBaseKey()}.{key}", GetCurrentSaveFilePath());
    }

    public static void SaveData<T>(string key, T data)
    {
        ES3.Save($"{GetBaseKey()}.{key}", data, GetCurrentSaveFilePath());
    }

    public static T LoadData<T>(string key, T defaultValue = default)
    {
        return ES3.Load($"{GetBaseKey()}.{key}", GetCurrentSaveFilePath(), defaultValue);
    }

    private static string GetBaseKey()
    {
        return MethodBase.GetCurrentMethod().DeclaringType.Namespace;
    }

    private static string GetCurrentSaveFilePath()
    {
        return GameNetworkManager.Instance.currentSaveFileName;
    }
}
