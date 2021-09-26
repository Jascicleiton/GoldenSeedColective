using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int gameYear = 33;
    private Season gameSeason = Season.Spring;
    private int gameDay = 24;
    private int gameHour = 7;
    private int gameMinute = 0;
    private int gameSecond = 0;
    private string gameDayOfWeek;

    private bool gameClockPaused = false;
    private float gameTick = 0f;

    protected override void Awake()
    {
        base.Awake();

        gameDayOfWeek = GetDayOfWeek();
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;
        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;
        if (gameSecond == Settings.secondsToMinute)
        {
            gameSecond = 0;
            UpdateGameMinute();
        }
    }

    private void UpdateGameMinute()
    {
        gameMinute++;
        if (gameMinute == Settings.minutesToHour)
        {
            gameMinute = 0;
            UpdateGameHour();
        }

        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void UpdateGameHour()
    {
        gameHour++;
        if (gameHour == Settings.hoursToDay)
        {
            gameHour = 0;
            UpdateGameDay();
        }

        EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void UpdateGameDay()
    {
        gameDay++;
        if (gameDay == Settings.daysToSeason + 1)
        {
            gameDay = 1;
            UpdateGameSeason();
        }
        gameDayOfWeek = GetDayOfWeek();
        EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void UpdateGameSeason()
    {
        int gs = (int)gameSeason;
        gs++;
        gameSeason = (Season)gs;
        if (gs == Settings.seasonsToYear)
        {
            gs = 0;
            gameSeason = (Season)gs;
            UpdateGameYear();
        }
        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void UpdateGameYear()
    {
        gameYear++;
        if(gameYear >= Settings.maxYear)
        {
            gameYear = 1;
        }
        EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)gameSeason) * Settings.daysToSeason) + gameDay;
        int dayOfWeek = totalDays % Settings.daysToWeek;
        switch (dayOfWeek)
        {
            case 0:
                return DayOfWeek.Sun.ToString();
            case 1:
                return DayOfWeek.Mon.ToString();
            case 2:
                return DayOfWeek.Tue.ToString();
            case 3:
                return DayOfWeek.Wed.ToString();
            case 4:
                return DayOfWeek.Thu.ToString();
            case 5:
                return DayOfWeek.Fri.ToString();
            case 6:
                return DayOfWeek.Sat.ToString();
            default:
                Debug.LogWarning("TIMEMANAGER: Need to update GetDayOfWeek function");
                return "";
        }
    }

    //TODO: Remove
    /// <summary>
    /// Advance 1 game minute
    /// </summary>
    public void TestAdvanceGameMinute()
    {
        UpdateGameMinute();
    }

    //TODO: Remove
    /// <summary>
    /// Advance 1 game days
    /// </summary>
    public void TestAdvanceGameDay()
    {
        UpdateGameDay();
    }

    //TODO: Remove
    /// <summary>
    /// Advance 1 game season
    /// </summary>
    public void TestAdvanceGameSeason()
    {
        for (int i = 0; i < 31; i++)
        {
            UpdateGameDay();
        }
    }
}
