using Model;
namespace UnitTests;

public class GameTests
{
    [Fact]
    public void TestGameConstructor()
    {
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        var game = new Game(players);

        Assert.Equal(2, game.PlayerCount);
        Assert.Equal(players[0], game.CurrentPlayer);
        Assert.Equal(0, game.CardPassed);
        Assert.IsType<ClassicRules>(game.Rules);
        Assert.NotNull(game.Deck);
    }
    
    [Fact]
    public void TestNextPlayer()
    {
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2"),
        };
        var game = new Game(players);

        Assert.Equal(players[0], game.CurrentPlayer);
        
        game.NextPlayer();
        Assert.Equal(players[1], game.CurrentPlayer);
        
        game.NextPlayer();
        Assert.Equal(players[0], game.CurrentPlayer);
    }
    
    [Fact]
    public void TestCardPassed()
    {
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        var game = new Game(players);

        Assert.Equal(0, game.CardPassed);
        
        game.CardPassed = 5;
        Assert.Equal(5, game.CardPassed);
    }
    
}
