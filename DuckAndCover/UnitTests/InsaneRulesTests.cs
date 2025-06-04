using Models.Game;
using Models.Enums;
using Models.Rules;
using Models.Exceptions;

namespace UnitTests;

public class InsaneRulesTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        var rules = new InsaneRules();
        Assert.Equal("Règles Insanes", rules.Name);
        Assert.NotEmpty(rules.Description);
        Assert.Equal(128, rules.NbCardsInDeck);
    }

    [Fact]
    public void Duck_InvalidMove_IfOccupied()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 2);
        grid.GameCardsGrid.Add(new GameCard(3, 8) { Position = newPos });
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<ErrorException>(() => rules.TryValidMove(card.Position, newPos, grid, "duck", deckCard));
    }

    [Fact]
    public void Duck_ValidMove_IfEmptyAndAdjacent()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 2);
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        rules.TryValidMove(card.Position, newPos, grid, "duck", deckCard);
        Assert.True(true);
    }

    [Fact]
    public void Cover_InvalidMove_IfEmpty()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 2);
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<ErrorException>(() => rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard));
    }

    [Fact]
    public void Cover_ValidMove_DoesNotThrowException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear();
        var card1 = new GameCard(7, 5) { Position = new Position(1, 1) };
        var card2 = new GameCard(3, 5) { Position = new Position(1, 2) };
        grid.GameCardsGrid.Add(card1);
        grid.GameCardsGrid.Add(card2);
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        rules.TryValidMove(card1.Position, card2.Position, grid, "cover", deckCard);
        Assert.True(true);
    }

    [Fact]
    public void Cover_InvalidCardNumber_ThrowsException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear();
        var card = new GameCard(7, 2) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 2);
        grid.GameCardsGrid.Add(new GameCard(3, 13) { Position = newPos });
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 8);
        var ex = Assert.Throws<ErrorException>(() =>
            rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard));
        Assert.Equal(ErrorCodes.CardNumberNotEqualToDeckCardNumber, ex.ErrorCode);
    }

    [Fact]
    public void Cover_InvalidMove_ThrowsError()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(10, 10);
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<ErrorException>(() =>
            rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard)
        );
    }

    [Fact]
    public void GameOver_CardPassed16()
    {
        var rules = new InsaneRules();
        Assert.True(rules.IsGameOver(16, 5, false));
    }

    [Fact]
    public void GameOver_Stack7()
    {
        var rules = new InsaneRules();
        Assert.True(rules.IsGameOver(0, 7, false));
    }

    [Fact]
    public void GameOver_NotOver()
    {
        var rules = new InsaneRules();
        Assert.False(rules.IsGameOver(1, 2, false));
    }

    [Fact]
    public void GameOver_Quit()
    {
        var rules = new InsaneRules();
        Assert.True(rules.IsGameOver(0, 0, true));
    }

    [Fact]
    public void SameCard_True()
    {
        var rules = new InsaneRules();
        var c1 = new GameCard(7, 5);
        var c2 = new DeckCard(Bonus.None, 5);
        Assert.True(rules.isTheSameCard(c1, c2));
    }

    [Fact]
    public void SameCard_False()
    {
        var rules = new InsaneRules();
        var c1 = new GameCard(4, 1);
        var c2 = new DeckCard(Bonus.None, 2);
        Assert.False(rules.isTheSameCard(c1, c2));
    }

    [Fact]
    public void TryValidMove_CardNotFound_ThrowsException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear(); // Clear default cards
        var position = new Position(1, 1);
        var newPosition = new Position(1, 2);
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5);

        // Don't add any card at the position - this will trigger CardNotFound

        var ex = Assert.Throws<ErrorException>(() => 
            rules.TryValidMove(position, newPosition, grid, "duck", deckCard));
        
        Assert.Equal(ErrorCodes.CardNotFound, ex.ErrorCode);
    }

    [Fact]
    public void TryValidMove_CardNumberNotEqualToDeckCardNumber_ThrowsException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear(); // Clear default cards
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPosition = new Position(1, 2);
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 8); // Different number

        var ex = Assert.Throws<ErrorException>(() => 
            rules.TryValidMove(card.Position, newPosition, grid, "duck", deckCard));
        
        Assert.Equal(ErrorCodes.CardNumberNotEqualToDeckCardNumber, ex.ErrorCode);
    }

    [Fact]
    public void Duck_CardAlreadyExists_ThrowsException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear(); // Clear default cards
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPosition = new Position(1, 2);
        // Add a card at the new position to trigger CardAlreadyExists
        grid.GameCardsGrid.Add(new GameCard(3, 8) { Position = newPosition });
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5); // Same number as the card

        var ex = Assert.Throws<ErrorException>(() => 
            rules.TryValidMove(card.Position, newPosition, grid, "duck", deckCard));
        
        Assert.Equal(ErrorCodes.CardAlreadyExists, ex.ErrorCode);
    }

    [Fact]
    public void Cover_CardNotFound_ThrowsException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear(); // Clear default cards
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPosition = new Position(1, 2); // Don't add any card at this position
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5); // Same number as the card

        var ex = Assert.Throws<ErrorException>(() => 
            rules.TryValidMove(card.Position, newPosition, grid, "cover", deckCard));
        
        Assert.Equal(ErrorCodes.CardNotFound, ex.ErrorCode);
    }

    [Fact]
    public void TryValidMove_InvalidFunctionName_ThrowsException()
    {
        var grid = new Grid();
        grid.GameCardsGrid.Clear(); // Clear default cards
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPosition = new Position(1, 2);
        // Add a card at new position for cover scenario
        grid.GameCardsGrid.Add(new GameCard(3, 8) { Position = newPosition });
        var rules = new InsaneRules();
        var deckCard = new DeckCard(Bonus.None, 5); // Same number as the card

        var ex = Assert.Throws<ErrorException>(() => 
            rules.TryValidMove(card.Position, newPosition, grid, "invalid", deckCard));
        
        Assert.Equal(ErrorCodes.InvalidFunctionName, ex.ErrorCode);
    }
} 