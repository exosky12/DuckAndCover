using Models.Enums;
using Models.Game;
using Models.Exceptions;
using Models.Rules;

namespace UnitTests;

public class BotTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        Bot bot = new Bot("007");

        Assert.Equal("Bot#007", bot.Name);
        Assert.True(bot.IsBot);
        Assert.False(bot.HasSkipped);
        Assert.Empty(bot.Scores);
        Assert.Equal(0, bot.TotalScore);
        Assert.NotNull(bot.Grid);
        Assert.Equal(12, bot.StackCounter);
    }

    

    [Fact]
    public void PlayTurnAutomatically_ExecutesAction()
    {
        Bot bot = new Bot("1");
        Game game = new Game(new ClassicRules());
        game.Players = new List<Player> { bot };
        game.CurrentDeckCard = new DeckCard(Bonus.None, 5);

        game.CurrentPlayer = bot;

        var position = new Position(1, 1);
        var card = new GameCard(3, 5) { Position = position };
        bot.Grid.GameCardsGrid.Add(card);

        var dummyTarget = new GameCard(3, 2) { Position = new Position(1, 2) };
        bot.Grid.GameCardsGrid.Add(dummyTarget);

        Bot.PlayTurnAutomatically(game);

        Assert.False(bot.HasSkipped); 
    }

    
}
