using System.Collections.ObjectModel;
using Models.Game;
using Models.Rules;
using Models.Exceptions;
using Models.Enums;
namespace UnitTests;

public class GameTests
{
    [Fact]
    public void TestGameConstructor()
    {
        var players = new ObservableCollection<Player>
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

            var players = new ObservableCollection<Player>
            {
                new Player("Alice"),
                new Player("Bob")
            };
            string id = "ABCDE";
            int currentPlayerIndex = 1;
            int cardsSkipped = 2;
            bool isFinished = true;

            var game = new Game(id, players, currentPlayerIndex, cardsSkipped, isFinished, new Deck(), null);

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
        var players = new ObservableCollection<Player>
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
        var players = new ObservableCollection<Player>
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
        var players = new ObservableCollection<Player> { new Player("Test") };
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
        var players = new ObservableCollection<Player> { new Player("Test") };
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
        var players = new ObservableCollection<Player> { 
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
        var players = new ObservableCollection<Player> { 
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
        var players = new ObservableCollection<Player> { new Player("Test") };
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
        public void NextDeckCard_RemovesCardAndUpdatesCurrent()
        {
            // Arrange
            var players = new ObservableCollection<Player>
            {
                new Player("Player1", 0, new List<int>(), false, false, new Grid()),
                new Player("Player2", 0, new List<int>(), false, false, new Grid())
            };

            var game = new Game(players);
            var initialDeckCount = game.Deck.Cards.Count;
            var initialFirstCard = game.Deck.Cards[0];
            var expectedNextCard = game.Deck.Cards[1];

            // Act
            var next = game.NextDeckCard();

            // Assert
            Assert.Equal(expectedNextCard, next);
            Assert.Equal(expectedNextCard, game.CurrentDeckCard);
            Assert.Equal(initialDeckCount - 1, game.Deck.Cards.Count);
            Assert.DoesNotContain(initialFirstCard, game.Deck.Cards);
        }

        [Fact]
        public void NextDeckCard_Throws_WhenDeckIsEmpty()
        {
            // Arrange
            var players = new ObservableCollection<Player>
            {
                new Player("Player1", 0, new List<int>(), false, false, new Grid()),
                new Player("Player2", 0, new List<int>(), false, false, new Grid())
            };

            var game = new Game(players);
            game.Deck.Cards.Clear(); // vider le deck

            // Act & Assert
            var ex = Assert.Throws<Error>(() => game.NextDeckCard());
            Assert.Equal(ErrorCodes.DeckEmpty, ex.ErrorCode);
        }
        private Game SetupSimpleGame()
        {
            var player1 = new Player("Test", 0, new List<int>(), false, false, new Grid());
            var player2 = new Player("Bot", 0, new List<int>(), false, false, new Grid());
            return new Game(new ObservableCollection<Player> { player1, player2 });
        }

        [Fact]
        public void HandlePlayerChooseCover_Throws_WhenInvalid()
        {
            var game = SetupSimpleGame();
            var player = game.CurrentPlayer;

            var from = new Position(0, 0); // a priori invalide
            var to = new Position(4, 4);   // a priori invalide

            Assert.Throws<Error>(() => game.HandlePlayerChooseCover(player, from, to));
        }

        [Fact]
        public void HandlePlayerChooseDuck_Throws_WhenInvalid()
        {
            var game = SetupSimpleGame();
            var player = game.CurrentPlayer;

            var from = new Position(0, 0); // position invalide
            var to = new Position(3, 3);   // aussi

            Assert.Throws<Error>(() => game.HandlePlayerChooseDuck(player, from, to));
        }

        [Fact]
        public void TriggerGameOver_RaisesGameIsOverEvent()
        {
            var game = SetupSimpleGame();
            bool triggered = false;

            game.GameIsOver += (s, e) => triggered = true;

            game.TriggerGameOver();

            Assert.True(triggered);
        }

        [Fact]
        public void CheckGameOverCondition_ReturnsTrue_AndRaisesEvent()
        {
            var game = SetupSimpleGame();
            bool eventRaised = false;

            game.GameIsOver += (s, e) => eventRaised = true;

            game.Quit = true;

            var result = game.CheckGameOverCondition();

            Assert.True(result);
            Assert.True(eventRaised);
        }

        [Fact]
        public void CheckGameOverCondition_ReturnsFalse_WhenNotOver()
        {
            var game = SetupSimpleGame();

            game.CardsSkipped = 0;
            game.CurrentPlayer.StackCounter = 5;
            game.Quit = false;

            var result = game.CheckGameOverCondition();

            Assert.False(result);
        }

    [Fact]
    public void PlayerChooseCoin_HandlesCoinCorrectly()
    {
        var players = new ObservableCollection<Player> { new Player("Test") };
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
