using System.Security.Cryptography;
using Models.Interfaces;
using Models.Game;
using Models.Enums;

namespace Models.Generators
{
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
            Deck.Clear();
            List<DeckCard> copy = new List<DeckCard>(AllPossibleCards);

            while (copy.Count > 0)
            {
                int index = GetSecureRandomIndex(copy.Count);
                Deck.Add(copy[index]);
                copy.RemoveAt(index);
            }

            return Deck;
        }

        private static int GetSecureRandomIndex(int max)
        {
            return RandomNumberGenerator.GetInt32(max);
        }
    }
}