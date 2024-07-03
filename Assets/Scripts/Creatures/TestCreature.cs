using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreature : Creature
{
    private void Start()
    {
        Init();
    }

    public override void DoMove(Vector2Int position)
    {
        Debug.Log($"Should move {position}");
    }

    public override void DoBrace()
    {
        Debug.Log("Should brace.");
    }

    public override void DoCapture()
    {
        Debug.Log("Do capture");
    }

    public override void DoAttack(Creature target)
    {
        Debug.Log($"Do attack {target.Name}");
    }

    public override void Die()
    {
        Debug.Log($"Do die.");
    }
}
