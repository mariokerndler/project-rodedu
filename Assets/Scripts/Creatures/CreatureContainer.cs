using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureContainer", menuName = "Creatures/CreatureContainer", order = 1)]
public class CreatureContainer : ScriptableObject
{
    public List<CreatureStrength> CreatureStrengths = new List<CreatureStrength>();
    
    [Serializable]
    public struct CreatureStrength
    {
        public int StrengthLevel;
        public List<Creature> Creatures;
    }
}