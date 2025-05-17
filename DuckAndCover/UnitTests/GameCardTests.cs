using Models.Game;
namespace UnitTests;

public class GameCardTests
{
    [Fact]
    public void TestCardConstructor()
    {
        GameCard card = new GameCard(4, 12);
        
        Assert.Equal(4, card.Splash);
        Assert.Equal(12, card.Number);
        Assert.Equal(new Position(0, 0), card.Position);
    }
    
    [Fact]
    public void TestCardNewPosition()
    {
        GameCard card = new GameCard(4, 12);
        Position newPosition = new Position(3, 3);
        
        card.Position = newPosition;
        
        Assert.Equal(newPosition, card.Position);
    }
}