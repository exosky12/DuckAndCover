using Model;
namespace UnitTests;

public class PlayerTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        Player player = new Player("Alice");

        Assert.Equal("Alice", player.Name);
        Assert.False(player.HasPassed);
        Assert.Empty(player.Scores);
        Assert.Equal(0, player.TotalScore);
        Assert.NotNull(player.Grid);
        Assert.Equal(0, player.StackCounter);
    }

    [Fact]
    public void HasCardWithNumber_ReturnsTrue_IfCardExists()
    {
        Player player = new Player("Bob");
        Grid grid = player.Grid;   
        grid.GameCardsGrid.Add(new GameCard(7, 5));

        bool result = player.HasCardWithNumber(5);

        Assert.True(result);
    }

    [Fact]
    public void HasCardWithNumber_ReturnsFalse_IfCardDoesNotExist()
    {
        Player player = new Player("Bob");
        player.Grid.GameCardsGrid.Add(new GameCard(2, 2));

        bool result = player.HasCardWithNumber(75);

        Assert.False(result);
    }

    [Fact]
    public void NextPlayer_CyclesThroughPlayersCorrectly()
    {
        Player player1 = new Player("Alice");
        Player player2 = new Player("Bob");
        Player player3 = new Player("Charlie");

        var players = new List<Player> { player1, player2, player3 };
        var game = new Game(players);

        Assert.Equal(player1, game.CurrentPlayer);

        player1.CallCoin(game, player1.Grid);
        Assert.Equal(player2, game.CurrentPlayer);

        player2.CallCoin(game, player2.Grid);
        Assert.Equal(player3, game.CurrentPlayer);

        player3.CallCoin(game, player3.Grid);
        Assert.Equal(player1, game.CurrentPlayer); 
    }

    [Fact]
    public void Cover_CoversCardCorrectly()
    {
        Player player = new Player("Bob");
        Game game = new Game(new List<Player> { player });
        Grid grid = player.Grid;
        GameCard aboveCard = new GameCard(5, 1) { Position = new Position(1, 1) };
        GameCard belowCard = new GameCard(5, 2) { Position = new Position(1, 2) };

        grid.GameCardsGrid.Add(aboveCard);
        grid.GameCardsGrid.Add(belowCard);

        bool result = player.Cover(aboveCard, belowCard, grid, game);

        Assert.True(result);
        Assert.Equal(new Position(1, 2), aboveCard.Position);
        Assert.Contains(aboveCard, grid.GameCardsGrid);
    }

    [Fact]
    public void Duck_MoveCardSuccess()
    {
        Player player = new Player("Bob");
        Game game = new Game(new List<Player> { player });
        Grid grid = player.Grid;
        GameCard card = new GameCard(3, 7) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        Position newPos = new Position(1, 2);
        
        bool result = player.Duck(card, newPos, grid, game);
        
        Assert.True(result);
        Assert.Equal(newPos, card.Position);
    }


    [Fact]
    public void Duck_DoesNotMoveCardIfNotAdjacent()
    {
        Player player = new Player("Bob");
        Game game = new Game(new List<Player> { player });
        Grid grid = player.Grid;
        GameCard card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        Position newPos = new Position(6, 6);
        
        bool result = player.Duck(card, newPos, grid, game);
        
        Assert.False(result);
        Assert.Equal(new Position(1, 1), card.Position);
    }

}
