using Models.Game;
using Models.Enums;
using Models.Rules;
using Models.Exceptions;

namespace UnitTests;

public class BlitzRulesTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        var rules = new BlitzRules();
        Assert.Equal("Règles Blitz", rules.Name);
        Assert.NotEmpty(rules.Description);
        Assert.Equal(24, rules.NbCardsInDeck);
    }

    [Fact]
    public void GameOver_CardPassed2()
    {
        var rules = new BlitzRules();
        Assert.True(rules.IsGameOver(2, 5, false));
    }

    [Fact]
    public void GameOver_Stack5()
    {
        var rules = new BlitzRules();
        Assert.True(rules.IsGameOver(0, 5, false));
    }

    [Fact]
    public void GameOver_NotOver()
    {
        var rules = new BlitzRules();
        Assert.False(rules.IsGameOver(1, 2, false));
    }

    [Fact]
    public void GameOver_Quit()
    {
        var rules = new BlitzRules();
        Assert.True(rules.IsGameOver(0, 0, true));
    }

} 