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
}