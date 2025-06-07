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
            currentDeckCard: deck.Cards.FirstOrDefault() ?? throw new ErrorException(ErrorCodes.DeckEmpty)
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
            currentDeckCard: deck.Cards.FirstOrDefault() ?? throw new ErrorException(ErrorCodes.DeckEmpty)
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
            Assert.NotNull(args.ErrorException);
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
        game.StartGame();

        bool menuDisplayed = false;
        game.PlayerChooseShowPlayersGrid += (sender, args) =>
        {
            menuDisplayed = true;
            Assert.NotNull(args.Players);
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "4");
        Assert.True(menuDisplayed);
        Assert.Equal(GameStateEnum.WaitingForPlayerAction, game.GameState.CurrentState);
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
        game.StartGame();

        int gridsDisplayed = 0;
        game.PlayerChooseShowPlayersGrid += (sender, args) =>
        {
            Assert.Equal(2, args.Players.Count);
            gridsDisplayed = args.Players.Count;
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "4");
        Assert.Equal(2, gridsDisplayed);
        Assert.Equal(GameStateEnum.WaitingForPlayerAction, game.GameState.CurrentState);
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
        game.StartGame();

        bool scoresDisplayed = false;
        game.PlayerChooseShowScores += (sender, args) =>
        {
            scoresDisplayed = true;
            Assert.Equal(2, args.Players.Count);
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "5");
        Assert.True(scoresDisplayed);
        Assert.Equal(GameStateEnum.WaitingForPlayerAction, game.GameState.CurrentState);
    }

    [Fact]
    public void PlayerChooseQuit_HandlesQuitCorrectly()
    {
        var players = new List<Player> { new Player("Test") };
        var game = new Game(new ClassicRules());
        game.Players = players;
        game.StartGame();

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

        var ex = Assert.Throws<ErrorException>(() => game.NextDeckCard());
        Assert.Equal(ErrorCodes.DeckEmpty, ex.ErrorCode);
    }

    private static Game SetupSimpleGame()
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

        Assert.Throws<ErrorException>(() => game.HandlePlayerChooseCover(player, from, to));
    }

    [Fact]
    public void HandlePlayerChooseDuck_Throws_WhenInvalid()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;

        var from = new Position(0, 0);
        var to = new Position(3, 3);

        Assert.Throws<ErrorException>(() => game.HandlePlayerChooseDuck(player, from, to));
    }

    [Fact]
    public void TriggerGameOver_RaisesGameIsOverEvent()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        bool triggered = false;

        game.GameIsOver += (s, e) => triggered = true;
        game.TriggerGameOver();

        Assert.True(triggered);
        Assert.Equal(GameStateEnum.GameOver, game.GameState.CurrentState);
    }

    [Fact]
    public void CheckGameOverCondition_ReturnsTrue_AndRaisesEvent()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        bool eventRaised = false;

        game.GameIsOver += (s, e) => eventRaised = true;
        game.Quit = true;

        var result = game.CheckGameOverCondition();

        Assert.True(result);
        Assert.True(eventRaised);
        Assert.Equal(GameStateEnum.GameOver, game.GameState.CurrentState);
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
        game.StartGame();

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

        int result = Game.GetEffectiveDeckCardNumber(player, card);

        Assert.Equal(20, result);
    }

    [Fact]
    public void GetEffectiveDeckCardNumber_Throws_WhenDeckCardIsNull()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;

        Assert.Throws<InvalidOperationException>(() => Game.GetEffectiveDeckCardNumber(player, null));
    }

    [Fact]
    public void GetEffectiveDeckCardNumber_ReturnsCardNumber_WhenBonusNotMax()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;
        var card = new DeckCard(Bonus.None, 5);

        int result = Game.GetEffectiveDeckCardNumber(player, card);

        Assert.Equal(5, result);
    }

    [Fact]
    public void ProcessCardEffect_HandlesMaxBonus_WithEmptyGrid()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;
        var card = new DeckCard(Bonus.Max, 5);
        bool effectProcessed = false;

        game.CardEffectProcessed += (s, e) =>
        {
            effectProcessed = true;
            Assert.Contains("MAX", e.Message);
        };

        game.ProcessCardEffect(card, player);

        Assert.True(effectProcessed);
    }

    [Fact]
    public void ProcessTurn_HandlesAllPlayersPlayed()
    {
        var game = SetupSimpleGame();
        game.Players.ForEach(p => p.HasPlayed = true);
        bool playerChanged = false;

        game.PlayerChanged += (s, e) => playerChanged = true;

        game.ProcessTurn();

        Assert.True(playerChanged);
        Assert.All(game.Players, p => Assert.False(p.HasPlayed));
        Assert.All(game.Players, p => Assert.False(p.HasSkipped));
    }


    [Fact]
    public void HandlePlayerChoice_Throws_WhenNotPlayerTurn()
    {
        var game = SetupSimpleGame();
        var otherPlayer = new Player("Other", 0, new List<int>(), false, false, new Grid());
        bool errorOccurred = false;

        game.ErrorOccurred += (s, e) => errorOccurred = true;

        game.HandlePlayerChoice(otherPlayer, "1");

        Assert.True(errorOccurred);
    }

    [Fact]
    public void GetValidDuckTargetPositions_ReturnsEmpty_WhenOnlyOneCard()
    {
        var game = SetupSimpleGame();
        var player = game.CurrentPlayer;
        var card = new GameCard(1, 1);
        var position = new Position(1, 1);
        player.Grid.SetCard(position, card);
        player.Grid.GameCardsGrid.Clear();
        player.Grid.GameCardsGrid.Add(card);

        var result = Game.GetValidDuckTargetPositions(player, position, new DeckCard(Bonus.None, 1));

        Assert.Empty(result);
    }

    [Fact]
    public void SaveGame_UpdatesExistingGame()
    {
        var game = new Game(new ClassicRules());
        game.Id = "test";
        game.Games = new ObservableCollection<Game> { game };
        game.CardsSkipped = 5;
        game.LastGameFinishStatus = true;
        game.LastNumber = 42;

        game.SaveGame();

        var savedGame = game.Games.First(g => g.Id == "test");
        Assert.Equal(5, savedGame.CardsSkipped);
        Assert.True(savedGame.LastGameFinishStatus);
        Assert.Equal(42, savedGame.LastNumber);
    }

    [Fact]
    public void ProcessCardEffect_MaxBonus_WithCardsInGrid()
    {
        var player = new Player("P1", 0, new List<int>(), false, false, new Grid());
        player.Grid.GameCardsGrid.Add(new GameCard(1, 10));
        player.Grid.GameCardsGrid.Add(new GameCard(2, 15));
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player };
        var card = new DeckCard(Bonus.Max, 5);

        bool effectProcessed = false;
        game.CardEffectProcessed += (s, e) =>
        {
            effectProcessed = true;
            Assert.Contains("MAX", e.Message);
            Assert.Contains("plus élevé", e.Message);
        };

        game.ProcessCardEffect(card, player);

        Assert.True(effectProcessed);
    }

    [Fact]
    public void ProcessCardEffect_DefaultCase_WithZeroNumber()
    {
        var player = new Player("P1", 0, new List<int>(), false, false, new Grid());
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player };
        var card = new DeckCard(Bonus.Again, 0);

        bool effectProcessed = false;
        game.CardEffectProcessed += (s, e) =>
        {
            effectProcessed = true;
            Assert.Contains("Again", e.Message);
        };

        game.ProcessCardEffect(card, player);

        Assert.True(effectProcessed);
    }

    [Fact]
    public void ProcessCardEffect_DefaultCase_WithNormalNumber()
    {
        var player = new Player("P1", 0, new List<int>(), false, false, new Grid());
        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player };
        var card = new DeckCard(Bonus.None, 7);

        bool effectProcessed = false;
        game.CardEffectProcessed += (s, e) =>
        {
            effectProcessed = true;
            Assert.Contains("Carte actuelle du deck : 7", e.Message);
        };

        game.ProcessCardEffect(card, player);

        Assert.True(effectProcessed);
        Assert.Equal(7, game.LastNumber);
    }

    [Fact]
    public void NextDeckCard_ThrowsWhenEmptyAfterRemoval()
    {
        var game = SetupSimpleGame();
        game.Deck.Cards.Clear();
        game.Deck.Cards.Add(new DeckCard(Bonus.None, 1)); // Only one card

        var ex = Assert.Throws<ErrorException>(() => game.NextDeckCard());
        Assert.Equal(ErrorCodes.DeckEmpty, ex.ErrorCode);
        Assert.Contains("Plus de cartes dans le deck", ex.Message);
    }

    [Fact]
    public void StartGame_DoesNotStartTwice()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        
        int playerChangedCount = 0;
        game.PlayerChanged += (s, e) => playerChangedCount++;
        
        game.StartGame(); // Should not start again
        
        Assert.Equal(0, playerChangedCount);
    }

    [Fact]
    public void StartGame_ThrowsWhenNoDeckCards()
    {
        var game = SetupSimpleGame();
        game.Deck.Cards.Clear();
        game.CurrentDeckCard = null!;

        bool errorOccurred = false;
        game.ErrorOccurred += (s, e) =>
        {
            errorOccurred = true;
            Assert.Equal(ErrorCodes.DeckEmpty, e.ErrorException.ErrorCode);
        };

        game.StartGame();

        Assert.True(errorOccurred);
    }

    [Fact]
    public void StartGame_SetsCurrentDeckCardFromDeck()
    {
        var game = SetupSimpleGame();
        game.CurrentDeckCard = null!;
        var firstCard = game.Deck.Cards.First();

        game.StartGame();

        Assert.Equal(firstCard, game.CurrentDeckCard);
    }

    [Fact]
    public void HandlePlayerChoice_Case1_CoverEvent()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        bool coverEventRaised = false;

        game.PlayerChooseCover += (s, e) =>
        {
            coverEventRaised = true;
            Assert.Equal(game.CurrentPlayer, e.Player);
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "1");

        Assert.True(coverEventRaised);
        Assert.Equal(GameStateEnum.WaitingForCoverTarget, game.GameState.CurrentState);
    }

    [Fact]
    public void HandlePlayerChoice_Case2_DuckEvent()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        bool duckEventRaised = false;

        game.PlayerChooseDuck += (s, e) =>
        {
            duckEventRaised = true;
            Assert.Equal(game.CurrentPlayer, e.Player);
        };

        game.HandlePlayerChoice(game.CurrentPlayer, "2");

        Assert.True(duckEventRaised);
        Assert.Equal(GameStateEnum.WaitingForDuckTarget, game.GameState.CurrentState);
    }

    [Fact]
    public void HandlePlayerChoice_Case6_QuitAndCheckGameOver()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        game.Quit = true;
        bool gameOverEventRaised = false;

        game.GameIsOver += (s, e) => gameOverEventRaised = true;

        game.HandlePlayerChoice(game.CurrentPlayer, "6");

        Assert.True(gameOverEventRaised);
        Assert.Equal(GameStateEnum.GameOver, game.GameState.CurrentState);
    }

    [Fact]
    public void HandlePlayerChooseCover_NotPlayerTurn_ThrowsError()
    {
        var game = SetupSimpleGame();
        var otherPlayer = new Player("Other", 0, new List<int>(), false, false, new Grid());

        var ex = Assert.Throws<ErrorException>(() => 
            game.HandlePlayerChooseCover(otherPlayer, new Position(1, 1), new Position(1, 2)));
        
        Assert.Equal(ErrorCodes.NotPlayerTurn, ex.ErrorCode);
    }

    [Fact]
    public void HandlePlayerChooseDuck_NotPlayerTurn_ThrowsError()
    {
        var game = SetupSimpleGame();
        var otherPlayer = new Player("Other", 0, new List<int>(), false, false, new Grid());

        var ex = Assert.Throws<ErrorException>(() => 
            game.HandlePlayerChooseDuck(otherPlayer, new Position(1, 1), new Position(1, 2)));
        
        Assert.Equal(ErrorCodes.NotPlayerTurn, ex.ErrorCode);
    }

    [Fact]
    public void GetValidDuckTargetPositions_ReturnsEmpty_WhenNullPlayer()
    {
        var result = Game.GetValidDuckTargetPositions(null!, new Position(1, 1), new DeckCard(Bonus.None, 1));
        Assert.Empty(result);
    }

    [Fact]
    public void GetValidDuckTargetPositions_ReturnsEmpty_WhenNullDeckCard()
    {
        var player = new Player("Test", 0, new List<int>(), false, false, new Grid());
        var result = Game.GetValidDuckTargetPositions(player, new Position(1, 1), null!);
        Assert.Empty(result);
    }

    [Fact]
    public void GetValidDuckTargetPositions_ReturnsEmpty_WhenCardNotFound()
    {
        var player = new Player("Test", 0, new List<int>(), false, false, new Grid());
        player.Grid.GameCardsGrid.Clear();
        var result = Game.GetValidDuckTargetPositions(player, new Position(1, 1), new DeckCard(Bonus.None, 1));
        Assert.Empty(result);
    }

    [Fact]
    public void GetValidDuckTargetPositions_ReturnsValidTargets()
    {
        var player = new Player("Test", 0, new List<int>(), false, false, new Grid());
        player.Grid.GameCardsGrid.Clear();
        
        var card1 = new GameCard(1, 1) { Position = new Position(1, 1) };
        var card2 = new GameCard(2, 2) { Position = new Position(1, 2) };
        player.Grid.GameCardsGrid.Add(card1);
        player.Grid.GameCardsGrid.Add(card2);

        var result = Game.GetValidDuckTargetPositions(player, new Position(1, 1), new DeckCard(Bonus.None, 1));
        
        Assert.NotEmpty(result);
    }

    [Fact]
    public void SavePlayers_SkipsBotPlayers()
    {
        var humanPlayer = new Player("Human", 0, new List<int>(), false, false, new Grid());
        var botPlayer = new Bot("Bot1");
        
        humanPlayer.Grid.GameCardsGrid.Add(new GameCard(1, 1));
        botPlayer.Grid.GameCardsGrid.Add(new GameCard(2, 2));

        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { humanPlayer, botPlayer };
        game.AllPlayers = new ObservableCollection<Player>();

        game.SavePlayers();

        Assert.Contains(humanPlayer, game.AllPlayers);
        Assert.DoesNotContain(botPlayer, game.AllPlayers);
        Assert.True(humanPlayer.Scores.Count > 0);
        Assert.Empty(botPlayer.Scores);
    }

    [Fact]
    public void SavePlayers_UpdatesExistingPlayer()
    {
        var player = new Player("Test", 0, new List<int>(), false, false, new Grid());
        player.Grid.GameCardsGrid.Add(new GameCard(1, 1));
        
        var existingPlayer = new Player("Test", 0, new List<int>(), false, false, new Grid());
        existingPlayer.Scores.Add(5);

        var game = new Game(new ClassicRules());
        game.Players = new List<Player> { player };
        game.AllPlayers = new ObservableCollection<Player> { existingPlayer };

        game.SavePlayers();

        Assert.Equal(2, existingPlayer.Scores.Count);
        Assert.Single(game.AllPlayers);
    }

    [Fact]
    public void ProcessTurn_HandlesErrorOccurred()
    {
        var game = SetupSimpleGame();
        game.Deck.Cards.Clear(); // This will cause an error in NextDeckCard
        game.Players.ForEach(p => p.HasPlayed = true); // Force NextDeckCard call

        bool errorOccurred = false;
        game.ErrorOccurred += (s, e) =>
        {
            errorOccurred = true;
            Assert.Equal(ErrorCodes.DeckEmpty, e.ErrorException.ErrorCode);
        };

        game.ProcessTurn();

        Assert.True(errorOccurred);
    }

    [Fact]
    public void ProcessTurn_ChecksGameOverAfterNextDeckCard()
    {
        var game = SetupSimpleGame();
        game.StartGame();
        game.Players.ForEach(p => p.HasPlayed = true);
        game.CardsSkipped = 8; // This will trigger game over in classic rules

        bool gameOverTriggered = false;
        game.GameIsOver += (s, e) => gameOverTriggered = true;

        game.ProcessTurn();

        Assert.True(gameOverTriggered);
        Assert.Equal(GameStateEnum.GameOver, game.GameState.CurrentState);
    }

    [Fact]
    public void ProcessTurn_ThrowsWhenCurrentDeckCardIsNull()
    {
        var game = SetupSimpleGame();
        game.CurrentDeckCard = null!;

        bool errorOccurred = false;
        game.ErrorOccurred += (s, e) =>
        {
            errorOccurred = true;
            Assert.Equal(ErrorCodes.DeckEmpty, e.ErrorException.ErrorCode);
            Assert.Contains("Plus de cartes disponibles", e.ErrorException.Message);
        };

        game.ProcessTurn();

        Assert.True(errorOccurred);
    }
}