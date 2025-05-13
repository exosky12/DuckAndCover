using Model;
namespace UnitTests;

public class DeckCardTests
{
    [Fact]
    public void DeckCard_Constructor_WithNumberAndNoBonus()
    {
        int number = 5;

        DeckCard card = new DeckCard(number);

        Assert.Equal(number, card.Number);
        Assert.Equal(Bonus.None, card.Bonus);
    }
    
    [Fact]
    public void DeckCard_Constructor_WithBonusAndNumber()
    {
        Bonus bonus = Bonus.Max;
        int number = 10;

        DeckCard card = new DeckCard(bonus, number);

        Assert.Equal(number, card.Number);
        Assert.Equal(bonus, card.Bonus);
    }
    
}