namespace Model;

public enum Bonus
{
    Max,
    Again
}

public class DeckCard : Card
{
    public Bonus Bonus { get; private set; }

    public DeckCard(Bonus bonus, int number) : base(number)
    {
        this.Bonus = bonus;
    }
}