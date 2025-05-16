using Models.Enums;
using Models.Generators;

namespace Models.Game
{
    public class Deck
    {
        public List<DeckCard> Cards { get; private set; }

        public Deck()
        {
            var generator = new DeckGenerator();
            Cards = generator.Generate();
            if (Cards[0].Bonus == Bonus.Again)
            {
                DeckCard tmp = Cards[0];
                Cards.RemoveAt(0);
                Cards.Add(tmp);
            }
        }
    }
}