using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public struct CreatureStrength
{
    public int strengthLevel;
    public List<Creature> creatures;
}