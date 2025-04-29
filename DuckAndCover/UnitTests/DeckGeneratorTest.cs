using Model;

namespace UnitTests
{
    public class DeckGeneratorTests
    {
        [Fact]
        public void Generate()
        {
            var initialCards = new List<DeckCard>
            {
                new DeckCard(Bonus.Max, 1),
                new DeckCard(Bonus.Max, 2),
                new DeckCard(Bonus.Max, 3),
                new DeckCard(Bonus.Max, 4),
            };

            var generator = new DeckGenerator()
            {
                NbCards = initialCards.Count
            };

            generator.AllPossibleCards.Clear();
            generator.AllPossibleCards.AddRange(initialCards);

            var snapshot = new List<DeckCard>(generator.AllPossibleCards);


            var deck = generator.Generate();


            Assert.Equal(initialCards.Count, deck.Count);


            Assert.Equal(deck.Count, deck.Distinct().Count());


            Assert.All(deck, card => Assert.Contains(card, initialCards));


            Assert.Equal(snapshot, generator.AllPossibleCards);
        }
    }
}