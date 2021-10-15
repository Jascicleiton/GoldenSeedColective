using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void OnEnable()
    {
        EventHandler.GameOver += GameOver;
    } 

    private void OnDisable()
    {
        EventHandler.GameOver -= GameOver;
    }

    private void GameOver()
    {
        print("GAME OVER");
    }


}
