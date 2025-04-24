namespace Model;

public class DeckGenerator
{
    public List<DeckCard> Deck { get; private set; } = new List<DeckCard>();

    /* ici faudra mettre une liste de cartes possibles pour le jeu (le deck du jeu) */
    public List<DeckCard> AllPossibleCards { get; private set; } = new List<DeckCard>();

    public int NbCards = 52;

    public List<Card> GenerateDeck()
    {
        var rand = new Random();
        while (Deck.Count < NbCards)
        {
            var card = AllPossibleCards[rand.Next(1, AllPossibleCards.Count)];
            Deck.Add(card);
            AllPossibleCards.Remove(card);
        }

        return Deck;
    }
}