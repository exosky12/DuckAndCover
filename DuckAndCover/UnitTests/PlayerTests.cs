using System.Collections.ObjectModel;
using Models.Game;
using Models.Exceptions;
namespace UnitTests;

public class PlayerTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        Player player = new Player("Alice");

        Assert.Equal("Alice", player.Name);
        Assert.False(player.HasSkipped);
        Assert.Empty(player.Scores);
        Assert.Equal(0, player.TotalScore);
        Assert.NotNull(player.Grid);
        Assert.Equal(12, player.StackCounter);
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

        var players = new ObservableCollection<Player> { player1, player2, player3 };
        var game = new Game(players);

        Assert.Equal(player1, game.CurrentPlayer);

        game.DoCoin(player1);
        Assert.Equal(player2, game.CurrentPlayer);

        game.DoCoin(player2);
        Assert.Equal(player3, game.CurrentPlayer);

        game.DoCoin(player3);
        Assert.Equal(player1, game.CurrentPlayer); 
    }

    [Fact]
    public void Cover_CoversCardCorrectly()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player>{ player });
        DeckCard deckCard = game.CurrentDeckCard;
        Grid grid = player.Grid;
        
        Position cardToMovePos = new Position(1, 1);
        grid.RemoveCard(cardToMovePos);
        GameCard cardToMove = new GameCard(5, deckCard.Number) { Position = cardToMovePos };
        
        Position cardToCoverPos = new Position(1, 2);
        grid.RemoveCard(cardToCoverPos);
        GameCard cardToCover = new GameCard(5, 2) { Position = cardToCoverPos };

        grid.GameCardsGrid.Add(cardToMove);
        grid.GameCardsGrid.Add(cardToCover);

        try 
        {
            game.DoCover(player, cardToMove.Position, cardToCover.Position);
        }
        catch (Error e)
        {
            Assert.IsType<Error>(e);
        }

        Assert.Equal(new Position(1, 2), cardToMove.Position);
    }

    [Fact]
    public void Duck_MoveCardSuccess()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player> { player });
        Grid grid = player.Grid;
        DeckCard deckCard = game.CurrentDeckCard;
        GameCard card = new GameCard(3, deckCard.Number) { Position = new Position(1, 1) };
        grid.RemoveCard(card.Position);
        grid.GameCardsGrid.Add(card);
        Position newPos = new Position(1, 5);
        
        try 
        {
            game.DoDuck(player, card.Position, newPos);
        }
        catch (Error e)
        {
            Assert.IsType<Error>(e);
        }
        
        Assert.Equal(newPos, card.Position);
    }

    [Fact]
    public void Duck_DoesNotMoveCardIfNotAdjacent()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player> { player });
        Grid grid = player.Grid;
        GameCard card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        Position newPos = new Position(6, 6);
        
        Assert.Throws<Error>(() => game.DoDuck(player, card.Position, newPos));
        Assert.Equal(new Position(1, 1), card.Position);
    }

    [Fact]
    public void Cover_ThrowsError_WhenInvalidPosition()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player> { player });
        Grid grid = player.Grid;
        GameCard cardToMove = new GameCard(5, 1) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(cardToMove);

        Assert.Throws<Error>(() => game.DoCover(player, new Position(1, 1), new Position(999, 999)));
    }

    [Fact]
    public void Duck_ThrowsError_WhenInvalidPosition()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player> { player });
        Grid grid = player.Grid;
        GameCard card = new GameCard(3, 7) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);

        Assert.Throws<Error>(() => game.DoDuck(player, new Position(1, 1), new Position(999, 999)));
    }

    [Fact]
    public void Cover_UpdatesStackCounter()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player> { player });
        DeckCard deckCard = game.CurrentDeckCard;
        Grid grid = player.Grid;
        
        Position cardToMovePos = new Position(1, 1);
        grid.RemoveCard(cardToMovePos);
        GameCard cardToMove = new GameCard(5, deckCard.Number) { Position = cardToMovePos };
        
        Position cardToCoverPos = new Position(1, 2);
        grid.RemoveCard(cardToCoverPos);
        GameCard cardToCover = new GameCard(5, 2) { Position = cardToCoverPos };

        grid.GameCardsGrid.Add(cardToMove);
        grid.GameCardsGrid.Add(cardToCover);
        
        int initialStackCounter = player.StackCounter;

        try 
        {
            game.DoCover(player, cardToMove.Position, cardToCover.Position);
        }
        catch (Error e)
        {
            Assert.IsType<Error>(e);
        }
        

        Assert.Equal(initialStackCounter - 1, player.StackCounter);
    }

    [Fact]
    public void Duck_DoesNotUpdateStackCounter()
    {
        Player player = new Player("Bob");
        Game game = new Game(new ObservableCollection<Player> { player });
        Grid grid = player.Grid;
        GameCard card = new GameCard(3, 7) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        Position newPos = new Position(1, 2);

        int initialStackCounter = player.StackCounter;
        try 
        {
            game.DoDuck(player, card.Position, newPos);
        }
        catch (Error e)
        {
            Assert.IsType<Error>(e);
        }

        Assert.Equal(initialStackCounter, player.StackCounter);
    }
}
