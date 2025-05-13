using Model;

public class PlayerTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        var player = new Player("Alice");

        Assert.Equal("Alice", player.Name);
        Assert.Equal(0, player.GameScore);
        Assert.False(player.HasPassed);
        Assert.Empty(player.Scores);
        Assert.Equal(0, player.TotalScore);
        Assert.NotNull(player.Grid);
    }

    [Fact]
    public void HasCardWithNumber_ReturnsTrue_IfCardExists()
    {
        var player = new Player("Bob");
        Grid grid = player.Grid;   
        grid.GameCardsGrid.Add(new GameCard(7,5));

        bool result = player.HasCardWithNumber(5);

        Assert.True(result);
    }

    [Fact]
    public void HasCardWithNumber_ReturnsFalse_IfCardDoesNotExist()
    {
        var player = new Player("Bob");
        player.Grid.GameCardsGrid.Add(new GameCard(2,2));

        bool result = player.HasCardWithNumber(75);

        Assert.False(result);
    }

    [Fact]
    public void NextPlayer_CyclesThroughPlayersCorrectly()
    {
        var player1 = new Player("Alice");
        var player2 = new Player("Bob");
        var player3 = new Player("Charlie");

        var players = new List<Player> { player1, player2, player3 };
        var game = new Game(players);


        Assert.Equal(player1, game.CurrentPlayer);

        Player.CallCoin(game);
        Assert.Equal(player2, game.CurrentPlayer);

        Player.CallCoin(game);
        Assert.Equal(player3, game.CurrentPlayer);

        Player.CallCoin(game);
        Assert.Equal(player1, game.CurrentPlayer); 
    }
}

