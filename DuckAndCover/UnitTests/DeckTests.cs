using Models.Enums;
using Models.Game;
using Xunit;
using System.Linq;

namespace UnitTests
{
    public class DeckTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCards_NotEmpty()
        {
            var deck = new Deck();

            Assert.NotNull(deck.Cards);
            Assert.NotEmpty(deck.Cards);
        }

        [Fact]
        public void Constructor_ShouldMoveAgainBonusCardToEnd_IfFirstCardIsAgain()
        {
            var cards = new System.Collections.Generic.List<DeckCard>
            {
                new DeckCard(Bonus.Again, 1),
                new DeckCard(Bonus.None, 2),
                new DeckCard(Bonus.None, 3)
            };

            if (cards[0].Bonus == Bonus.Again)
            {
                var tmp = cards[0];
                cards.RemoveAt(0);
                cards.Add(tmp);
            }

            Assert.Equal(Bonus.None, cards[0].Bonus);
            Assert.Equal(Bonus.Again, cards.Last().Bonus);
        }
    }
}