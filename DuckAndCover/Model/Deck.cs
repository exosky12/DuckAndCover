namespace Model;

public class Deck
{
    public List<DeckCard> Cards { get; private set; }


    public Deck()
    {
        var generator = new DeckGenerator();
        Cards = generator.Generate();
    }
}