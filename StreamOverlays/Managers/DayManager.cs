using com.github.zehsteam.StreamOverlays.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.zehsteam.StreamOverlays.Managers;

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

        if (!GameSaveFileHelper.KeyExists("DayData"))
        {
            return;
        }

        try
        {
            string json = GameSaveFileHelper.Load<string>("DayData");
            DayDataList = JsonConvert.DeserializeObject<List<DayData>>(json);

            Logger.LogInfo($"Loaded day data from save file. {json}", extended: true);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to load day data from save file. {ex}");
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
            GameSaveFileHelper.Save("DayData", json);

            Logger.LogInfo($"Saved day data to save file. {json}", extended: true);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to save day data to save file. {ex}");
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

[Serializable]
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
