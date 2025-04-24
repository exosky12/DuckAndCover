namespace Model;

public class GenerateDeck : IGenerator
{
    public List<Card> Deck { get; private set; } = new List<Card>();

    /* ici faudra mettre une liste de cartes possibles pour le jeu (le deck du jeu) */
    public List<Card> AllPossibleCards { get; private set; } = new List<Card>();

    private int NbCards = 52;

    public List<Card> GenerateDeck()
    {
        var rand = new Random();
        while (Deck.Count < NbCards)
        {
            var card = AllPossibleCards[rand.Next(1, AllPossibleCards.Count)];
            if (card is DeckCard)
            {
                Deck.Add(card);
                AllPossibleCards.Remove(card);
            }
        }

        return Deck;
    }
}