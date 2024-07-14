using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerData PlayerData { get; private set; }

    public CreatureContainer CreatureContainer;

    public MouseController MouseController;
    
    private void Start()
    {
        StartGame();
    }
    
    // TODO: Turn this into a coroutine, to wait for placement finish
    public void StartGame()
    {
        SetupPlayer();
        
        MapManager.Instance.LoadMap(0);
        MapManager.Instance.LoadCreatures(PlayerData.Room, PlayerData.Floor);
        
        PlayerPlacesCreatures();
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

    private void SetupPlayer()
    {
        PlayerData = new PlayerData();
        
        // TODO: Implement creature choosing.
        foreach (var creatureStrength in CreatureContainer.CreatureStrengths)
        {
            PlayerData.Creatures.Add(creatureStrength.Creatures.First());
        }
    }

    private void PlayerPlacesCreatures()
    {
        var tiles = MapManager.Instance.GetFilteredTiles(true);
        tiles.ForEach(x => x.ShowTile());

        MouseController.EnablePlayerPlacement(PlayerData.Creatures, tiles);
    }

    private Queue<Creature> CreateTurnOrder()
    {
        return new Queue<Creature>();
    }

}
