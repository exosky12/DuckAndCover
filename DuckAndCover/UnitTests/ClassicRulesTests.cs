using Model;

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
        bool result = rules.IsValidMove(card.Position, newPos, grid, "duck");

        Assert.False(result);
    }

    [Fact]
    public void Duck_ValidMove_IfEmptyAndAdjacent()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 5);

        var rules = new ClassicRules();
        bool result = rules.IsValidMove(card.Position, newPos, grid, "duck");

        Assert.True(result);
    }

    [Fact]
    public void Cover_InvalidMove_IfEmpty()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 5);

        var rules = new ClassicRules();
        bool result = rules.IsValidMove(card.Position, newPos, grid, "cover");

        Assert.False(result);
    }

    [Fact]
    public void Cover_ValidMove_IfOccupied()
    {
        var grid = new Grid();
        var card = new GameCard(7, 5) { Position = new Position(1, 1) };
        grid.GameCardsGrid.Add(card);
        var newPos = new Position(1, 2);

        grid.GameCardsGrid.Add(new GameCard(3, 8) { Position = newPos });

        var rules = new ClassicRules();
        bool result = rules.IsValidMove(card.Position, newPos, grid, "cover");

        Assert.True(result);
    }
    
    [Fact]
    public void GameOver_CardPassed8()
    {
        var rules = new ClassicRules();
        Assert.True(rules.IsGameOver(8, 5));
    }

    [Fact]
    public void GameOver_Stack1()
    {
        var rules = new ClassicRules();
        Assert.True(rules.IsGameOver(5, 1));
    }

    [Fact]
    public void GameOver_NotOver()
    {
        var rules = new ClassicRules();
        Assert.False(rules.IsGameOver(3, 4));
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
}