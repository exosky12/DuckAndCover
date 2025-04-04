namespace ClassLibrary
{
    public class Deck
    {
        public List<DeckCard> Cards { get; set; }
        public int Index { get; set; }

        public Deck()
        {
            Cards = new List<DeckCard>();
            Index = 0;
        }

        public DeckCard GetCard(int index) => Cards[index];
        public void AddCard(DeckCard card) => Cards.Add(card);

        public void ResetDeck()
        {
            /* TODO */
        }
    }
}