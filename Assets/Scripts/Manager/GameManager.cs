using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        StartGame();
    }
    
    public void StartGame()
    {
        MapManager.Instance.LoadMap(0);
        MapManager.Instance.LoadCreatures(1, 1);
    }

    public void LoadNextRoom()
    {

    }

    public void LoadNextFloor()
    {
        
    }

    public void GameOver()
    {
        
    }
}
