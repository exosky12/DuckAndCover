namespace Model;

public class Deck
{
    public List<DeckCard> Cards;

    public Deck()
    {
        var generator = new DeckGenerator();
        Cards = generator.Generate();
    }
}