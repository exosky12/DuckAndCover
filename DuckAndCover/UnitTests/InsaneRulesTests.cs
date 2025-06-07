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
    public void ValidateCoverMove_AllowsNonAdjacentCover()
    {
        var rules = new InsaneRules();
        var grid = new Grid();
        var currentPos = new Position(1, 1);
        var targetPos = new Position(5, 5);

        grid.SetCard(currentPos, new GameCard(1, 1));
        grid.SetCard(targetPos, new GameCard(2, 1));

        var exception = Record.Exception(() => rules.TryValidMove(currentPos, targetPos, grid, "cover", new DeckCard(Bonus.None, 1)));
        Assert.Null(exception);
    }

    [Fact]
    public void ValidateCoverMove_ThrowsWhenTargetEmpty()
    {
        var rules = new InsaneRules();
        var grid = new Grid();
        var currentPos = new Position(1, 1);
        var targetPos = new Position(10, 10);

        grid.SetCard(currentPos, new GameCard(1, 1));

        var ex = Assert.Throws<ErrorException>(() => 
            rules.TryValidMove(currentPos, targetPos, grid, "cover", new DeckCard(Bonus.None, 1)));
        Assert.Equal(ErrorCodes.CardNotFound, ex.ErrorCode);
    }

    [Fact]
    public void ValidateCoverMove_AllowsLongDistanceCover()
    {
        var rules = new InsaneRules();
        var grid = new Grid();
        var currentPos = new Position(0, 0);
        var targetPos = new Position(8, 8);

        grid.SetCard(currentPos, new GameCard(1, 1));
        grid.SetCard(targetPos, new GameCard(2, 1));

        var exception = Record.Exception(() => rules.TryValidMove(currentPos, targetPos, grid, "cover", new DeckCard(Bonus.None, 1)));
        Assert.Null(exception);
    }
} 