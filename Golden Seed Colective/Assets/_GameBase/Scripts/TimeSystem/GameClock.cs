using UnityEngine;
using TMPro;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText, dateText, seasonText, yearText;

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Update time. Minutes are incremented on a 10 basis
        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = "";
        string minute;

        if (gameHour >= Settings.hoursToDay / 2)
        {
            ampm = " pm";
        }
        else
        {
            ampm = " am";
        }
        if (gameHour >= Settings.hoursToDay / 2 + 1)
        {
            gameHour -= Settings.hoursToDay / 2;
        }
        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        string time = gameHour.ToString() + " : " + minute + ampm;

        timeText.SetText(time);
        dateText.SetText(gameDayOfWeek + ". " + gameDay.ToString());
        seasonText.SetText(gameSeason.ToString());
        yearText.SetText("Year " + gameYear);
    }
}
