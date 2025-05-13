namespace Model;

public class DeckGenerator : IGenerator<DeckCard>
{
    public List<DeckCard> Deck { get; private set; } = new List<DeckCard>();

    public List<DeckCard> AllPossibleCards { get; private set; } = InitializeDeck();

    private static List<DeckCard> InitializeDeck()
    {
        var cards = new List<DeckCard>();

        for (int i = 0; i < 2; i++)
        {
            for (int number = 1; number <= 12; number++)
            {
                cards.Add(new DeckCard(number));
            }
        }

        cards.Add(new DeckCard(Bonus.Max, 0));

        cards.Add(new DeckCard(Bonus.Again, 0));

        return cards;
    }
    

    public DeckGenerator()
    {
        Generate();
    }

    public List<DeckCard> Generate()
    {
        var rand = new Random();

        while (AllPossibleCards.Count > 0)
        {
            int index = rand.Next(AllPossibleCards.Count);
            var card = AllPossibleCards[index];

            Deck.Add(card);
            AllPossibleCards.RemoveAt(index);
        }

        return Deck;
    }
}