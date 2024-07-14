using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    private Queue<CreatureOwner> _turnOrder = new Queue<CreatureOwner>();
    private PlayerData _playerData;
    
    public void InitTurn(Queue<CreatureOwner> turnOrder, PlayerData playerData)
    {
        if (turnOrder.Count <= 0)
        {
            Debug.LogWarning("Turn order is empty.", this);
            return;
        }

        if (playerData is null)
        {
            Debug.LogError("Player data is null.", this);
            return;
        }
        
        this._turnOrder = turnOrder;
        this._playerData = playerData;
        
        var topElement = turnOrder.Peek();
        switch (topElement.Owner)
        {
            case EOwner.Enemy:
                StartCoroutine(StartEnemyTurn());
                break;
            case EOwner.Player:
                StartCoroutine(StartPlayerTurn());
                break;
        }
    }
    
    private IEnumerator StartPlayerTurn()
    {
        Debug.Log("Starting player turn");
        
        yield break;
    }

    private IEnumerator EndPlayerTurn()
    {
        yield break;
    }

    private IEnumerator StartEnemyTurn()
    {
        Debug.Log("Starting enemy turn");
        
        yield break;
    }

    private IEnumerator EndEnemyTurn()
    {
        yield break;
    }
}
