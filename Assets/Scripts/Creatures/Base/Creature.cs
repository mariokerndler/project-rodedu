using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }
    protected int CurrentHealth { get; set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int Armor { get; private set; }
    protected int CurrentAP { get; set; }
    [field: SerializeField] public int MaxAP { get; private set; }
    [field: SerializeField] public int Speed { get; private set; }
    
    [field: SerializeField] public ECreatureClass Class { get; private set; }
    [field: SerializeField] public ECreatureType Type { get; private set; }
    [field: SerializeField] public Card[] Cards { get; private set; }

    [HideInInspector] public OverlayTile standingOnTile;
    
    protected Creature() {}
    
    protected void Init()
    {
        CurrentHealth = MaxHealth;
        CurrentAP = MaxAP;
    }
    
    public abstract void DoMove(Vector2Int position);

    public abstract void DoBrace();

    public abstract void DoCapture();

    public abstract void DoAttack(Creature target);

    public abstract void Die();

    public virtual void ReceiveDamage(int amount)
    {
        CurrentHealth -= amount;
        
        if (CurrentHealth >= 0) return;
        
        CurrentHealth = 0;
        Die();
    }
}
