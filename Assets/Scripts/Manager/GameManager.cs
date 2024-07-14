using System;
using System.Collections;
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
    
    public void StartGame()
    {
        SetupPlayer();

        StartCoroutine(LoadNextRoom());
    }

    public IEnumerator LoadNextRoom()
    {
        // TODO: Change how map is chosen.
        MapManager.Instance.LoadMap(0);
        MapManager.Instance.LoadCreatures(PlayerData.Room, PlayerData.Floor);
        
        PlayerPlacesCreatures();

        yield return new WaitUntil(() => !MouseController.IsPlacingCreatures);
        
        var creatureQueue = CreateTurnOrder();
        TurnManager.Instance.InitTurn(creatureQueue, PlayerData);
        
        yield break;
    }

    public IEnumerator LoadNextFloor()
    {
        yield break;
    }

    private void GameOver()
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

    private Queue<CreatureOwner> CreateTurnOrder()
    {
        var enemyCreatures = MapManager.Instance.CurrentCreatures.Values.ToList();
        var playerCreatures = PlayerData.Creatures;

        var enemyCreatureOwner = enemyCreatures.Select(x => new CreatureOwner() { Owner = EOwner.Enemy, Creature = x })
            .ToList();
        var playerCreatureOwner = playerCreatures.Select(x => new CreatureOwner() { Owner = EOwner.Player, Creature = x })
            .ToList();

        var creatureOwnerList = new List<CreatureOwner>();
        creatureOwnerList.AddRange(enemyCreatureOwner);
        creatureOwnerList.AddRange(playerCreatureOwner);

        var sortedCreatureOwnerList = creatureOwnerList.OrderBy(co => co.Creature.Speed).ToList();
        var turnOrder = new Queue<CreatureOwner>();
        
        foreach (var creatureOwner in sortedCreatureOwnerList)
        {
            //Debug.Log($"Owner: {creatureOwner.Owner}, Creature Speed: {creatureOwner.Creature.Speed}");
            turnOrder.Enqueue(creatureOwner);
        }
        
        return turnOrder;
    }

}
