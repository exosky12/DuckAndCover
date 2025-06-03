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
        var players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        var game = new Game(new ClassicRules());
        Deck deck = new Deck();
        game.InitializeGame(
            id: "ED23F",
            players: players,
            deck: deck,
            currentDeckCard: deck.Cards.FirstOrDefault() ?? throw new Error(ErrorCodes.DeckEmpty)
        );

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
        var deck = new Deck();
        var currentDeckCard = deck.Cards.First();

        var game = new Game(new ClassicRules());
        game.InitializeGame(
            id: id,
            players: players,
            deck: deck,
            currentDeckCard: currentDeckCard,
            currentPlayerIndex: currentPlayerIndex,
            cardsSkipped: cardsSkipped,
            isFinished: isFinished
        );

        Assert.Equal(id, game.Id);
        Assert.Equal(players, game.Players);
        Assert.Equal(players[currentPlayerIndex], game.CurrentPlayer);
        Assert.Equal(cardsSkipped, game.CardsSkipped);
        Assert.True(game.IsFinished);
        Assert.Equal(currentDeckCard, game.CurrentDeckCard);
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
        var game = new Game(new ClassicRules());
        Deck deck = new Deck();
        game.InitializeGame(
            id: "12345",
            players: players,
            deck: deck,
            currentDeckCard: deck.Cards.FirstOrDefault() ?? throw new Error(ErrorCodes.DeckEmpty)
        );

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
        var game = new Game(new ClassicRules());
        game.Players = players;

        Assert.Equal(0, game.CardsSkipped);

        game.CardsSkipped = 5;
        Assert.Equal(5, game.CardsSkipped);
    }

    [Fact]
    public void ErrorOccurred_HandlesErrorCorrectly()
    {
        var players = new List<Player> { new Player("Test") };
        var game = new Game(new ClassicRules());
        game.Players = players;

        bool errorHandled = false;
        game.ErrorOccurred += (sender, args) =>
        {
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
        var game = new Game(new ClassicRules());
        game.Players = players;

        bool menuDisplayed = false;
        game.DisplayMenuNeeded += (sender, args) =>
        {
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
        var players = new List<Player>
        {
            new Player("Player1"),
            new Player("Player2")
        };
        var game = new Game(new ClassicRules());
        game.Players = players;

        int gridsDisplayed = 0;
        game.PlayerChooseShowPlayersGrid += (sender, args) =>
        {
            Assert.Equal(2, args.Players.Count);
            gridsDisplayed = args.Players.Count;
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "4");
        Assert.Equal(2, gridsDisplayed);
    }

    [Fact]
    public void PlayerChooseShowScores_DisplaysScores()
    {
        var players = new List<Player>
        {
            new Player("Player1"),
            new Player("Player2")
        };
        var game = new Game(new ClassicRules());
        game.Players = players;

        bool scoresDisplayed = false;
        game.PlayerChooseShowScores += (sender, args) =>
        {
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
        var game = new Game(new ClassicRules());
        game.Players = players;

        bool quitHandled = false;
        game.PlayerChooseQuit += (sender, args) =>
        {
            quitHandled = true;
            Assert.NotNull(args.Player);
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "6");
        Assert.True(quitHandled);
    }

    [Fact]
    public void NextDeckCard_RemovesCardAndUpdatesCurrent()
    {
        var players = new List<Player>
        {
            new Player("Player1", 0, new List<int>(), false, false, new Grid()),
            new Player("Player2", 0, new List<int>(), false, false, new Grid())
        };

        var game = new Game(new ClassicRules());
        game.Players = players;

        var initialDeckCount = game.Deck.Cards.Count;
        var initialFirstCard = game.Deck.Cards[0];
        var expectedNextCard = game.Deck.Cards[1];

        var next = game.NextDeckCard();

        Assert.Equal(expectedNextCard, next);
        Assert.Equal(expectedNextCard, game.CurrentDeckCard);
        Assert.Equal(initialDeckCount - 1, game.Deck.Cards.Count);
        Assert.DoesNotContain(initialFirstCard, game.Deck.Cards);
    }

    [Fact]
    public void NextDeckCard_Throws_WhenDeckIsEmpty()
    {
        var players = new List<Player>
        {
            new Player("Player1", 0, new List<int>(), false, false, new Grid()),
            new Player("Player2", 0, new List<int>(), false, false, new Grid())
        };

        var game = new Game(new ClassicRules());
        game.Players = players;
        game.Deck.Cards.Clear();

        var ex = Assert.Throws<Error>(() => game.NextDeckCard());
        Assert.Equal(ErrorCodes.DeckEmpty, ex.ErrorCode);
    }

    private Game SetupSimpleGame()
    {
        var player1 = new Player("Test", 0, new List<int>(), false, false, new Grid());
        var player2 = new Player("Bot", 0, new List<int>(), false, false, new Grid());
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player1, player2 };
        return game;
    }

    [Fact]
    public void HandlePlayerChooseCover_Throws_WhenInvalid()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;

        var from = new Position(0, 0);
        var to = new Position(4, 4);

        Assert.Throws<Error>(() => game.HandlePlayerChooseCover(player, from, to));
    }

    [Fact]
    public void HandlePlayerChooseDuck_Throws_WhenInvalid()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;

        var from = new Position(0, 0);
        var to = new Position(3, 3);

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
        var players = new List<Player> { new Player("Test") };
        var game = new Game(new ClassicRules());
        game.Players = players;

        bool coinHandled = false;
        game.PlayerChooseCoin += (sender, args) =>
        {
            coinHandled = true;
            Assert.NotNull(args.Player);
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "3");
        Assert.True(coinHandled);
    }
    
    [Fact]
    public void StartGame_InitializesGameAndRaisesPlayerChanged()
    {
        var players = new List<Player>
        {
            new Player("P1", 0, new List<int>(), false, false, new Grid()),
            new Player("P2", 0, new List<int>(), false, false, new Grid())
        };
        var game = new Game(new ClassicRules());
        game.Players = players;
        bool playerChanged = false;
        game.PlayerChanged += (s, e) => playerChanged = true;

        game.StartGame();

        Assert.True(game.IsGameStarted);
        Assert.True(playerChanged);
    }

    [Fact]
    public void ProcessCardEffect_AgainBonus_UsesLastNumber()
    {
        var player = new Player("P1", 0, new List<int>(), false, false, new Grid());
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player };
        game.LastNumber = 42;
        var card = new DeckCard(Bonus.Again, 5);

        bool effectProcessed = false;
        game.CardEffectProcessed += (s, e) =>
        {
            effectProcessed = true;
            Assert.Contains("Again", e.Message);
            Assert.Equal(42, e.ProcessedCard.Number);
        };

        game.ProcessCardEffect(card, player);

        Assert.True(effectProcessed);
    }

    [Fact]
    public void SavePlayers_AddsScoresToAllPlayers()
    {
        var player = new Player("P1", 0, new List<int>(), false, false, new Grid());
        player.Grid.GameCardsGrid.Add(new GameCard(1, 1));
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player };
        game.AllPlayers = new System.Collections.ObjectModel.ObservableCollection<Player>();

        game.SavePlayers();

        Assert.True(player.Scores.Count > 0);
        Assert.Contains(player, game.AllPlayers);
    }

    [Fact]
    public void SaveGame_UpdatesExistingGameOrAddsNew()
    {
        var game = new Game(new ClassicRules());
        game.Id = "test";
        game.Games = new System.Collections.ObjectModel.ObservableCollection<Game>();
        game.SaveGame();
        Assert.Contains(game, game.Games);
    }

    [Fact]
    public void CheckAllPlayersSkipped_ResetsHasSkippedAndIncrementsCardsSkipped()
    {
        var player1 = new Player("P1", 0, new List<int>(), false, false, new Grid()) { HasSkipped = true };
        var player2 = new Player("P2", 0, new List<int>(), false, false, new Grid()) { HasSkipped = true };
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player1, player2 };

        game.CheckAllPlayersSkipped();

        Assert.Equal(1, game.CardsSkipped);
        Assert.All(game.Players, p => Assert.False(p.HasSkipped));
    }

    [Fact]
    public void AllPlayersPlayed_ReturnsTrueIfAllPlayed()
    {
        var player1 = new Player("P1", 0, new List<int>(), false, false, new Grid());
        var player2 = new Player("P2", 0, new List<int>(), false, false, new Grid());
        player1.HasPlayed = true;
        player2.HasPlayed = true;
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player1, player2 };

        Assert.True(game.AllPlayersPlayed());
    }

    [Fact]
    public void GetEffectiveDeckCardNumber_ReturnsMaxIfBonusMax()
    {
        var player = new Player("P1", 0, new List<int>(), false, false, new Grid());
        player.Grid.GameCardsGrid.Add(new GameCard(1, 10));
        player.Grid.GameCardsGrid.Add(new GameCard(1, 20));
        var game = new Game(new ClassicRules());
        var card = new DeckCard(Bonus.Max, 5);

        int result = game.GetEffectiveDeckCardNumber(player, card);

        Assert.Equal(20, result);
    }
}