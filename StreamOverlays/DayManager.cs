using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace com.github.zehsteam.StreamOverlays;

internal static class DayManager
{
    private static List<DayData> _dayDataList = [];

    public static void LoadDayData()
    {
        _dayDataList = [];

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
            _dayDataList = JsonConvert.DeserializeObject<List<DayData>>(json);

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
            string json = JsonConvert.SerializeObject(_dayDataList);
            SaveSystem.SaveData("DayData", json);

            Plugin.Instance.LogInfoExtended($"Saved day data to save file. {json}");
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to save day data to save file. {ex}");
        }
    }

    public static void AddDayData(int day, int scrapCollected)
    {
        if (!CanAddDayData())
        {
            return;
        }

        if (_dayDataList.Any(x => x.Day == day))
        {
            return;
        }

        _dayDataList.Add(new DayData(day, scrapCollected));
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
        _dayDataList = [];
        SaveDayData();
    }

    public static int GetAveragePerDay()
    {
        if (_dayDataList.Count == 0)
        {
            return 0;
        }

        return _dayDataList.Sum(x => x.ScrapCollected) / _dayDataList.Count;
    }
}

[System.Serializable]
public struct DayData
{
    public int Day { get; private set; }
    public int ScrapCollected { get; private set; }

    public DayData(int day, int scrapCollected)
    {
        Day = day;
        ScrapCollected = scrapCollected;
    }
}
