using UnityEngine;

public class Card : ScriptableObject
{
    public string cardName;
    public int actionCost;
    public Sprite cardImage;
    public ECreatureClass @class;

    public virtual void PlayCard(Creature target)
    {
        
    }
}
