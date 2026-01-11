using com.github.zehsteam.StreamOverlays.Dependencies;
using com.github.zehsteam.StreamOverlays.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.zehsteam.StreamOverlays.Managers;

internal static class DayManager
{
    public static IReadOnlyList<DayData> DayDataList => _dayDataList;

    private static List<DayData> _dayDataList = [];

    public static void LoadDayData()
    {
        _dayDataList = [];

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
            _dayDataList = JsonConvert.DeserializeObject<List<DayData>>(json);

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

        if (StartOfRound.Instance == null || !StartOfRound.Instance.inShipPhase)
        {
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(_dayDataList);
            GameSaveFileHelper.Save("DayData", json);

            Logger.LogInfo($"Saved day data to save file. {json}", extended: true);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to save day data to save file. {ex}");
        }
    }

    public static void SetDayData(List<DayData> dayDataList)
    {
        _dayDataList = dayDataList;
    }

    public static void AddDayData(int scrapCollected)
    {
        if (!CanAddDayData())
        {
            return;
        }

        int dayNumber = GetDayNumber();

        if (_dayDataList.Any(x => x.Day == dayNumber))
        {
            return;
        }

        _dayDataList.Add(new DayData(dayNumber, scrapCollected));

        Logger.LogInfo($"\n\nDayManager.AddDayData() day: {dayNumber} scrapCollected: {scrapCollected}\n", extended: true);
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
        ResetDayData();
        SaveDayData();
    }

    public static void ResetDayData()
    {
        _dayDataList.Clear();
    }

    public static int GetDayNumber()
    {
        return _dayDataList.Count + 1;
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
