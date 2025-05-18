using Models.Game;
using Models.Rules;
namespace UnitTests;

public class GameTests
{
    [Fact]
    public void TestGameConstructor()
    {
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        var game = new Game(players);

        Assert.Equal(2, game.Players.Count);
        Assert.Equal(players[0], game.CurrentPlayer);
        Assert.Equal(0, game.CardsSkipped);
        Assert.IsType<ClassicRules>(game.Rules);
        Assert.NotNull(game.Deck);
    }

    [Fact]
        public void GameConstructor_WithAllParameters_InitializesCorrectly()
        {

            var players = new List<Player>
            {
                new Player("Alice"),
                new Player("Bob")
            };
            string id = "ABCDE";
            int currentPlayerIndex = 1;
            int cardsSkipped = 2;
            bool isFinished = true;

            var game = new Game(id, players, currentPlayerIndex, cardsSkipped, isFinished);

            Assert.Equal(id, game.Id);
            Assert.Equal(players, game.Players);
            Assert.Equal(players[currentPlayerIndex], game.CurrentPlayer);
            Assert.Equal(cardsSkipped, game.CardsSkipped);
            Assert.True(game.IsFinished);
            Assert.NotNull(game.CurrentDeckCard);
            Assert.IsType<ClassicRules>(game.Rules);
            Assert.NotNull(game.Deck);
        }

    
    [Fact]
    public void TestNextPlayer()
    {
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2"),
        };
        var game = new Game(players);

        Assert.Equal(players[0], game.CurrentPlayer);
        
        game.NextPlayer();
        Assert.Equal(players[1], game.CurrentPlayer);
        
        game.NextPlayer();
        Assert.Equal(players[0], game.CurrentPlayer);
    }
    
    [Fact]
    public void TestCardsSkipped()
    {
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        var game = new Game(players);

        Assert.Equal(0, game.CardsSkipped);
        
        game.CardsSkipped = 5;
        Assert.Equal(5, game.CardsSkipped);
    }
    
    [Fact]
    public void ErrorOccurred_HandlesErrorCorrectly()
    {
        var players = new List<Player> { new Player("Test") };
        var game = new Game(players);
        bool errorHandled = false;
        
        game.ErrorOccurred += (sender, args) => {
            errorHandled = true;
            Assert.NotNull(args.Error);
        };
        
        game.HandlePlayerChoice(game.CurrentPlayer, "invalid_choice");
        
        Assert.True(errorHandled);
    }

    [Fact]
    public void DisplayMenuNeeded_TriggersCorrectly()
    {
        var players = new List<Player> { new Player("Test") };
        var game = new Game(players);
        bool menuDisplayed = false;
        
        game.DisplayMenuNeeded += (sender, args) => {
            menuDisplayed = true;
            Assert.NotNull(args.CurrentPlayer);
            Assert.NotNull(args.DeckCard);
        };
        
        game.HandlePlayerChoice(game.CurrentPlayer, "4");
        
        Assert.True(menuDisplayed);
    }

    [Fact]
    public void PlayerChooseShowPlayersGrid_DisplaysAllGrids()
    {
        var players = new List<Player> { 
            new Player("Player1"),
            new Player("Player2")
        };
        var game = new Game(players);
        int gridsDisplayed = 0;
        
        game.PlayerChooseShowPlayersGrid += (sender, args) => {
            Assert.Equal(2, args.Players.Count);
            gridsDisplayed = args.Players.Count;
        };
        
        game.HandlePlayerChoice(game.CurrentPlayer, "4");
        
        Assert.Equal(2, gridsDisplayed);
    }

    [Fact]
    public void PlayerChooseShowScores_DisplaysScores()
    {
        var players = new List<Player> { 
            new Player("Player1"),
            new Player("Player2")
        };
        var game = new Game(players);
        bool scoresDisplayed = false;
        
        game.PlayerChooseShowScores += (sender, args) => {
            scoresDisplayed = true;
            Assert.Equal(2, args.Players.Count);
        };
        
        game.HandlePlayerChoice(game.CurrentPlayer, "5");
        
        Assert.True(scoresDisplayed);
    }

    [Fact]
    public void PlayerChooseQuit_HandlesQuitCorrectly()
    {
        var players = new List<Player> { new Player("Test") };
        var game = new Game(players);
        bool quitHandled = false;
        
        game.PlayerChooseQuit += (sender, args) => {
            quitHandled = true;
            Assert.NotNull(args.Player);
        };
        
        game.HandlePlayerChoice(game.CurrentPlayer, "6");
        
        Assert.True(quitHandled);
    }

    [Fact]
    public void PlayerChooseCoin_HandlesCoinCorrectly()
    {
        var players = new List<Player> { new Player("Test") };
        var game = new Game(players);
        bool coinHandled = false;
        
        game.PlayerChooseCoin += (sender, args) => {
            coinHandled = true;
            Assert.NotNull(args.Player);
        };
        
        game.HandlePlayerChoice(game.CurrentPlayer, "3");
        
        Assert.True(coinHandled);
    }
}
