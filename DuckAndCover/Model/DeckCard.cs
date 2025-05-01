namespace Model;

public enum Bonus
{
    None,
    Max,
    Again
}

public class DeckCard : Card
{
    public Bonus Bonus { get; private set; }

    public DeckCard(int number) : base(number)
    {
        this.Bonus = Bonus.None;
    }
    public DeckCard(Bonus bonus, int number) : base(number)
    {
        this.Bonus = bonus;
    }
}