using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameYear = 33;
    private Season gameSeason = Season.Spring;
    private int gameDay = 24;
    private int gameHour = 7;
    private int gameMinute = 0;
    private int gameSecond = 0;
    private string gameDayOfWeek;

   // private bool gameClockPaused = false;
    private float gameTick = 0f;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        gameDayOfWeek = GetDayOfWeek();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void Update()
    {
        //if (!gameClockPaused)
        //{
        //    GameTick();
        //}
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

    public string GetDayOfWeek()
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

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // Delete existing scene save if exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Create new int dictionary
        sceneSave.intDictionary = new Dictionary<string, int>();

        // Create new string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // Add values to the int dictionary
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);

        // Add values to the string dictionary
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        // Add scene save to game object for persistent scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // Get saved gameObject from gameSave data
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get savedScene data for  gameObject
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // if int and string dictionaries are found
                if(sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    // populate saved int values
                    if(sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                    {
                        gameYear = savedGameYear;
                    }
                    if(sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                    {
                        gameDay = savedGameDay;
                    }

                    // populate string saved values
                    if(sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                    {
                        gameDayOfWeek = savedGameDayOfWeek;
                    }

                    if(sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if(Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }
}
