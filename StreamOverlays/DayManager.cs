using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace com.github.zehsteam.StreamOverlays;

internal static class DayManager
{

    public static List<DayData> DayDataList = [];

    public static void LoadDayData()
    {
        DayDataList = [];

        if (!NetworkUtils.IsServer)
        {
            return;
        }

        if (!SaveSystem.KeyExists("DayData"))
        {
            return;
        }

        try
        {
            string json = SaveSystem.LoadData<string>("DayData");
            DayDataList = JsonConvert.DeserializeObject<List<DayData>>(json);

            Plugin.Instance.LogInfoExtended($"Loaded day data from save file. {json}");
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to load day data from save file. {ex}");
        }
    }

    public static void SaveDayData()
    {
        if (!NetworkUtils.IsServer)
        {
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(DayDataList);
            SaveSystem.SaveData("DayData", json);

            Plugin.Instance.LogInfoExtended($"Saved day data to save file. {json}");
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to save day data to save file. {ex}");
        }
    }

    public static void AddDayData(int scrapCollected)
    {
        if (!CanAddDayData())
        {
            return;
        }

        int dayNumber = GetDayNumber();

        if (DayDataList.Any(x => x.Day == dayNumber))
        {
            return;
        }

        DayDataList.Add(new DayData(dayNumber, scrapCollected));
    }

    private static bool CanAddDayData()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.currentLevel == null)
        {
            return false;
        }

        return StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap;
    }

    public static void ResetSavedDayData()
    {
        DayDataList = [];
        SaveDayData();
    }

    public static int GetDayNumber()
    {
        return DayDataList.Count + 1;
    }

    public static int GetAveragePerDay()
    {
        if (DayDataList.Count == 0)
        {
            return 0;
        }

        return DayDataList.Sum(x => x.ScrapCollected) / DayDataList.Count;
    }
}

[System.Serializable]
public struct DayData
{
    public int Day;
    public int ScrapCollected;

    public DayData(int day, int scrapCollected)
    {
        Day = day;
        ScrapCollected = scrapCollected;
    }
}
