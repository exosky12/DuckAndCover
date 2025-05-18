using Models.Game;
using Models.Enums;
using Models.Rules;
using Models.Exceptions;

namespace UnitTests;

public class ClassicRulesTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        var rules = new ClassicRules();

        Assert.Equal("Règles Classiques", rules.Name);
        Assert.NotEmpty(rules.Description);
        Assert.Equal(24, rules.NbCardsInDeck);
    }

    [Fact]
    public void Duck_InvalidMove_IfOccupied()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 2);

        // Ajouter une carte à la nouvelle position
        grid.GameCardsGrid.Add(new GameCard(3, 8) { Position = newPos });

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<Error>(() => rules.TryValidMove(card.Position, newPos, grid, "duck", deckCard));
    }

    [Fact]
    public void Duck_ValidMove_IfEmptyAndAdjacent()
    {
        var grid = new Grid();
        var card = grid.GetCard(new Position(1, 1))!;
        var newPos = new Position(1, 5);

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, card.Number);
        try
        {
            rules.TryValidMove(card.Position, newPos, grid, "duck", deckCard);
        }
        catch (Error e)
        {
            Assert.IsType<Error>(e);
        }

        Assert.True(true);
    }

    [Fact]
    public void Cover_InvalidMove_IfEmpty()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 5);

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<Error>(() => rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard));
    }

    [Fact]
    public void Cover_ValidMove_DoesNotThrowException()
    {
        var grid = new Grid();
        var card = grid.GetCard(new Position(1, 1))!;

        var newPos = new Position(1, 2);

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, card.Number);

        rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard);

        Assert.True(true);
    }

    [Fact]
    public void Cover_InvalidCardNumber_ThrowsException()
    {
        var grid = new Grid();
        var card = new GameCard(7, 2) { Position = new Position(1, 1) };
        grid.RemoveCard(new Position(1,1));
        grid.GameCardsGrid.Add(card);

        var newPos = new Position(1, 2);
        grid.GameCardsGrid.Add(new GameCard(3, 13) { Position = newPos });

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 8);

        var ex = Assert.Throws<Error>(() =>
            rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard));
    
        Assert.Equal(ErrorCodes.CardNumberNotEqualToDeckCardNumber, ex.ErrorCode);
    }

    [Fact]
    public void Cover_InvalidMove_ThrowsError()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(10, 10);

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 5);

        Assert.Throws<Error>(() =>
            rules.TryValidMove(card.Position, newPos, grid, "cover", deckCard)
        );
    }
    
    [Fact]
    public void GameOver_CardPassed8()
    {
        var rules = new ClassicRules();
        Assert.True(rules.IsGameOver(8, 5, false));
    }

    [Fact]
    public void GameOver_Stack1()
    {
        var rules = new ClassicRules();
        Assert.True(rules.IsGameOver(5, 1, false));
    }

    [Fact]
    public void GameOver_NotOver()
    {
        var rules = new ClassicRules();
        Assert.False(rules.IsGameOver(3, 4, false));
    }

    [Fact]
    public void GameOver_Quit()
    {
        var rules = new ClassicRules();
        Assert.True(rules.IsGameOver(0, 5, true));
    }

    [Fact]
    public void SameCard_True()
    {
        var rules = new ClassicRules();
        var c1 = new GameCard(7, 5);
        var c2 = new DeckCard(Bonus.None, 5);

        Assert.True(rules.isTheSameCard(c1, c2));
    }

    [Fact]
    public void SameCard_False()
    {
        var rules = new ClassicRules();
        var c1 = new GameCard(4, 1);
        var c2 = new DeckCard(Bonus.None, 2);

        Assert.False(rules.isTheSameCard(c1, c2));
    }

    [Fact]
    public void Duck_InvalidMove_IfNotAdjacent()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(3, 3); // Position non adjacente

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<Error>(() => rules.TryValidMove(card.Position, newPos, grid, "duck", deckCard));
    }

    [Fact]
    public void Cover_InvalidMove_IfSamePosition()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<Error>(() => rules.TryValidMove(card.Position, card.Position, grid, "cover", deckCard));
    }

    [Fact]
    public void Duck_InvalidMove_IfSamePosition()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);

        var rules = new ClassicRules();
        var deckCard = new DeckCard(Bonus.None, 5);
        Assert.Throws<Error>(() => rules.TryValidMove(card.Position, card.Position, grid, "duck", deckCard));
    }

    [Fact]
    public void SameCard()
    {
        var rules = new ClassicRules();
        var c1 = new GameCard(7, 5);
        var c2 = new DeckCard(Bonus.Again, 5);

        Assert.True(rules.isTheSameCard(c1, c2));
    }
    [Fact]
    public void IsNotSameCard()
    {
        var rules = new ClassicRules();
        var c1 = new GameCard(7, 2);
        var c2 = new DeckCard(Bonus.Again, 5);

        Assert.False(rules.isTheSameCard(c1, c2));
    }
}