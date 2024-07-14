using System.Collections.Generic;

public class PlayerData
{
    public List<Creature> Creatures { get; set; } = new();
    public List<Card> Cards { get; set; } = new();

    public int Floor { get; set; } = 1;
    public int Room { get; set; } = 1;
}