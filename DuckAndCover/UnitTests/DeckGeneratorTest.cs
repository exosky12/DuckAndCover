using Models.Game;
using Models.Enums;
using Models.Generators;

namespace UnitTests
{
    public class DeckGeneratorTests
    {
        [Fact]
        public void Generate_WithNormalCards()
        {
            var initialCards = new List<DeckCard>
            {
                new DeckCard(Bonus.None, 1),
                new DeckCard(Bonus.None, 2),
                new DeckCard(Bonus.None, 3),
                new DeckCard(Bonus.None, 4),
            };

            var generator = new DeckGenerator();
            generator.AllPossibleCards.Clear();
            generator.AllPossibleCards.AddRange(initialCards);

            var snapshot = new List<DeckCard>(generator.AllPossibleCards);
            var deck = generator.Generate();

            Assert.Equal(initialCards.Count, deck.Count);
            Assert.Equal(deck.Count, deck.Distinct().Count());
            Assert.All(deck, card => Assert.Contains(card, initialCards));
            Assert.Equal(snapshot, generator.AllPossibleCards);
        }

        [Fact]
        public void Generate_WithBonusCards()
        {
            var initialCards = new List<DeckCard>
            {
                new DeckCard(Bonus.Max, 1),
                new DeckCard(Bonus.Again, 2),
                new DeckCard(Bonus.None, 3),
                new DeckCard(Bonus.Max, 4),
            };

            var generator = new DeckGenerator();
            generator.AllPossibleCards.Clear();
            generator.AllPossibleCards.AddRange(initialCards);

            var deck = generator.Generate();

            Assert.Equal(initialCards.Count, deck.Count);
            Assert.Equal(deck.Count, deck.Distinct().Count());
            Assert.All(deck, card => Assert.Contains(card, initialCards));
        }

        [Fact]
        public void Generate_WithEmptyList()
        {
            var generator = new DeckGenerator();
            generator.AllPossibleCards.Clear();

            var deck = generator.Generate();

            Assert.Empty(deck);
        }

        [Fact]
        public void Generate_WithDefaultCards()
        {
            var generator = new DeckGenerator();
            var deck = generator.Generate();

            Assert.Equal(26, deck.Count);
            Assert.Equal(1, deck.Count(card => card.Bonus == Bonus.Max));
            Assert.Equal(1, deck.Count(card => card.Bonus == Bonus.Again));
            Assert.Equal(24, deck.Count(card => card.Bonus == Bonus.None));
        }
    }
}