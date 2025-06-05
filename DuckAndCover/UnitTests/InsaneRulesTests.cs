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
} 