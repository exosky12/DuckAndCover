using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Models.Exceptions;
using Models.Interfaces;
using Models.Events;
using Models.Enums;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Models.Game
{
    /// <summary>
    /// Représente une partie de jeu avec ses joueurs, son deck et sa logique.
    /// </summary>
    [DataContract]
    public class Game : INotifyPropertyChanged
    {
        /// <summary>
        /// Obtient ou définit l'identifiant unique de la partie.
        /// </summary>
        [DataMember]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Collection observable des joueurs de la partie.
        /// </summary>
        [DataMember] private ObservableCollection<Player> _allPlayers = new ObservableCollection<Player>();

        /// <summary>
        /// Obtient ou définit la collection observable des joueurs.
        /// </summary>
        public ObservableCollection<Player> AllPlayers
        {
            get => _allPlayers;
            set
            {
                _allPlayers = value;
                OnPropertyChanged(nameof(AllPlayers));
            }
        }

        /// <summary>
        /// Liste des joueurs de la partie.
        /// </summary>
        [DataMember]
        public List<Player> Players { get; set; } = new List<Player>();

        /// <summary>
        /// Collection observable des parties.
        /// </summary>
        [IgnoreDataMember] private ObservableCollection<Game> _games = new ObservableCollection<Game>();

        /// <summary>
        /// Obtient ou définit la collection observable des parties.
        /// </summary>
        public ObservableCollection<Game> Games
        {
            get => _games;
            set
            {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        /// <summary>
        /// Règles du jeu.
        /// </summary>
        public IRules Rules { get; set; }

        /// <summary>
        /// Nom des règles utilisées.
        /// </summary>
        [DataMember] private string _rulesName = string.Empty;

        /// <summary>
        /// Nombre de cartes passées.
        /// </summary>
        [DataMember]
        public int CardsSkipped { get; set; }

        /// <summary>
        /// Indique si l'événement de fin de partie a déjà été déclenché.
        /// </summary>
        [IgnoreDataMember] private bool _gameOverAlreadyTriggered = false;

        /// <summary>
        /// Joueur actuel.
        /// </summary>
        public Player CurrentPlayer { get; set; } = new Player("Default", 0, new List<int>(), false, false, new Grid());

        /// <summary>
        /// Index du joueur actuel.
        /// </summary>
        [DataMember] private int _currentPlayerIndex;

        /// <summary>
        /// Deck de cartes du jeu.
        /// </summary>
        [DataMember]
        public Deck Deck { get; set; } = new Deck();

        /// <summary>
        /// Indique si le jeu est quitté.
        /// </summary>
        public bool Quit { get; set; }

        /// <summary>
        /// Indique si le jeu est terminé.
        /// </summary>
        [DataMember]
        public bool IsFinished { get; set; }


        /// <summary>
        /// Statut de fin de la dernière partie.
        /// </summary>
        [DataMember]
        public bool LastGameFinishStatus { get; set; }

        /// <summary>
        /// Carte actuelle du deck.
        /// </summary>
        public DeckCard CurrentDeckCard { get; set; } = new DeckCard(Bonus.None, 0);

        /// <summary>
        /// Dernier numéro joué.
        /// </summary>
        [DataMember]
        public int? LastNumber { get; set; }

        /// <summary>
        /// Indique si le jeu a démarré.
        /// </summary>
        public bool IsGameStarted { get; private set; }

        private GameState _gameState;

        public GameState GameState => _gameState;

        public event EventHandler<PlayerChangedEventArgs>? PlayerChanged;
        public event EventHandler<GameIsOverEventArgs>? GameIsOver;
        public event EventHandler<ErrorOccurredEventArgs>? ErrorOccurred;
        public event EventHandler<PlayerChooseCoinEventArgs>? PlayerChooseCoin;
        public event EventHandler<PlayerChooseDuckEventArgs>? PlayerChooseDuck;
        public event EventHandler<PlayerChooseShowPlayersGridEventArgs>? PlayerChooseShowPlayersGrid;
        public event EventHandler<PlayerChooseQuitEventArgs>? PlayerChooseQuit;
        public event EventHandler<PlayerChooseCoverEventArgs>? PlayerChooseCover;
        public event EventHandler<PlayerChooseShowScoresEventArgs>? PlayerChooseShowScores;
        public event EventHandler<DisplayMenuNeededEventArgs>? DisplayMenuNeeded;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<CardEffectProcessedEventArgs>? CardEffectProcessed;

        protected virtual void OnPlayerChanged(PlayerChangedEventArgs args) => PlayerChanged?.Invoke(this, args);
        protected virtual void OnErrorOccurred(ErrorOccurredEventArgs args) => ErrorOccurred?.Invoke(this, args);
        protected virtual void OnGameIsOver(GameIsOverEventArgs args) => GameIsOver?.Invoke(this, args);

        protected virtual void OnPlayerChooseCoin(PlayerChooseCoinEventArgs args) =>
            PlayerChooseCoin?.Invoke(this, args);

        protected virtual void OnPlayerChooseDuck(PlayerChooseDuckEventArgs args) =>
            PlayerChooseDuck?.Invoke(this, args);

        protected virtual void OnDisplayMenuNeeded(DisplayMenuNeededEventArgs args) =>
            DisplayMenuNeeded?.Invoke(this, args);

        protected virtual void OnPlayerChooseQuit(PlayerChooseQuitEventArgs args) =>
            PlayerChooseQuit?.Invoke(this, args);

        protected virtual void OnPlayerChooseShowScores(PlayerChooseShowScoresEventArgs args) =>
            PlayerChooseShowScores?.Invoke(this, args);

        protected virtual void OnPlayerChooseShowPlayersGrid(PlayerChooseShowPlayersGridEventArgs args) =>
            PlayerChooseShowPlayersGrid?.Invoke(this, args);

        protected virtual void OnPlayerChooseCover(PlayerChooseCoverEventArgs args) =>
            PlayerChooseCover?.Invoke(this, args);

        protected virtual void OnCardEffectProcessed(CardEffectProcessedEventArgs args) =>
            CardEffectProcessed?.Invoke(this, args);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Game.
        /// </summary>
        /// <param name="rules">Les règles du jeu à utiliser.</param>
        public Game(IRules rules)
        {
            this.Rules = rules;
            this._rulesName = rules.GetType().Name;
            this._gameState = new GameState(this);
        }

        /// <summary>
        /// Initialise une nouvelle partie avec les paramètres spécifiés.
        /// </summary>
        /// <param name="id">L'identifiant de la partie.</param>
        /// <param name="players">La liste des joueurs.</param>
        /// <param name="deck">Le deck de cartes.</param>
        /// <param name="currentDeckCard">La carte actuelle du deck.</param>
        /// <param name="currentPlayerIndex">L'index du joueur actuel.</param>
        /// <param name="cardsSkipped">Le nombre de cartes passées.</param>
        /// <param name="isFinished">Indique si le jeu est terminé.</param>
        /// <param name="lastNumber">Le dernier numéro joué.</param>
        public void InitializeGame(string id, List<Player> players, Deck deck, DeckCard currentDeckCard,
            int currentPlayerIndex = 0, int cardsSkipped = 0, bool isFinished = false, int? lastNumber = null)
        {
            this.Id = id;
            this.Players = players;
            this.Deck = deck;
            this._currentPlayerIndex = currentPlayerIndex;
            this.CurrentPlayer = players[_currentPlayerIndex];
            this.CurrentDeckCard = currentDeckCard;
            this.CardsSkipped = cardsSkipped;
            this.IsFinished = isFinished;
            this.LastNumber = lastNumber;
        }

        /// <summary>
        /// Passe au joueur suivant.
        /// </summary>
        public void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
            CurrentPlayer = Players[_currentPlayerIndex];
        }

        /// <summary>
        /// Obtient le numéro effectif d'une carte du deck pour un joueur donné.
        /// </summary>
        /// <param name="forPlayer">Le joueur concerné.</param>
        /// <param name="deckCard">La carte du deck.</param>
        /// <returns>Le numéro effectif de la carte.</returns>
        public static int GetEffectiveDeckCardNumber(Player forPlayer, DeckCard? deckCard)
        {
            if (deckCard == null)
            {
                throw new InvalidOperationException("Deck card cannot be null to get effective number.");
            }

            if (deckCard.Bonus == Bonus.Max)
            {
                deckCard.Number = forPlayer.Grid.GameCardsGrid.Max(c => c.Number);
            }

            return deckCard.Number;
        }

        /// <summary>
        /// Vérifie si les conditions de fin de partie sont remplies.
        /// </summary>
        /// <returns>true si le jeu est terminé ; sinon, false.</returns>
        public bool CheckGameOverCondition()
        {
            Debug.WriteLine($"CheckGameOverCondition - CardsSkipped: {CardsSkipped}, StackCounter: {CurrentPlayer.StackCounter}, Quit: {Quit}");
            Debug.WriteLine($"CheckGameOverCondition - IsGameOver: {Rules.IsGameOver(CardsSkipped, CurrentPlayer.StackCounter, Quit)}");
            Debug.WriteLine($"CheckGameOverCondition - _gameOverAlreadyTriggered: {_gameOverAlreadyTriggered}");

            if (!_gameOverAlreadyTriggered && Rules.IsGameOver(CardsSkipped, CurrentPlayer.StackCounter, Quit))
            {
                Debug.WriteLine("CheckGameOverCondition - Game Over conditions met!");
                _gameOverAlreadyTriggered = true;
                IsFinished = true;
                LastGameFinishStatus = true;
                _gameState.TransitionTo(GameStateEnum.GameOver);
                OnGameIsOver(new GameIsOverEventArgs(true));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Traite l'effet d'une carte.
        /// </summary>
        /// <param name="card">La carte à traiter.</param>
        /// <param name="player">Le joueur concerné.</param>
        public void ProcessCardEffect(DeckCard card, Player player)
        {
            switch (card.Bonus)
            {
                case Bonus.Again when LastNumber.HasValue:
                    card.Number = LastNumber.Value;
                    OnCardEffectProcessed(new CardEffectProcessedEventArgs(
                        $"Carte Again active ! Le numéro utilisé est {card.Number}",
                        card));
                    break;

                case Bonus.Max:
                    string maxEffectMessage;
                    if (player.Grid.GameCardsGrid.Count > 0)
                    {
                        maxEffectMessage = $"Carte MAX ! Le numéro sera le plus élevé de la grille du joueur actif.";
                    }
                    else
                    {
                        maxEffectMessage =
                            $"Carte MAX ! Si la grille du joueur actif est vide, la carte prendra sa valeur de base ({card.Number}).";
                    }

                    OnCardEffectProcessed(new CardEffectProcessedEventArgs(maxEffectMessage, card));
                    break;

                default:
                    LastNumber = card.Number;
                    string msg = card.Number == 0 && card.Bonus != Bonus.None
                        ? card.Bonus.ToString()
                        : card.Number.ToString();
                    OnCardEffectProcessed(new CardEffectProcessedEventArgs(
                        $"Carte actuelle du deck : {msg}",
                        card));
                    break;
            }
        }

        /// <summary>
        /// Passe à la carte suivante du deck.
        /// </summary>
        /// <returns>La nouvelle carte du deck.</returns>
        public DeckCard NextDeckCard()
        {
            if (Deck.Cards.Count == 0)
                throw new ErrorException(ErrorCodes.DeckEmpty);

            Deck.Cards.RemoveAt(0);

            if (Deck.Cards.Count == 0)
            {
                CurrentDeckCard = null!;
                throw new ErrorException(ErrorCodes.DeckEmpty, "Plus de cartes dans le deck");
            }

            CurrentDeckCard = Deck.Cards.First();
            ProcessCardEffect(CurrentDeckCard, CurrentPlayer);

            return CurrentDeckCard;
        }

        /// <summary>
        /// Traite le tour de jeu actuel.
        /// </summary>
        public void ProcessTurn()
        {
            try
            {
                Debug.WriteLine("ProcessTurn - Starting");
                if (CheckGameOverCondition())
                {
                    Debug.WriteLine("ProcessTurn - Game Over detected at start");
                    return;
                }

                if (AllPlayersPlayed())
                {
                    Debug.WriteLine("ProcessTurn - All players played");
                    CheckAllPlayersSkipped();
                    NextDeckCard();
                    Players.ForEach(p =>
                    {
                        p.HasPlayed = false;
                        p.HasSkipped = false;
                    });
                }

                if (CheckGameOverCondition())
                {
                    Debug.WriteLine("ProcessTurn - Game Over detected after NextDeckCard");
                    return;
                }

                if (CurrentDeckCard == null)
                {
                    throw new ErrorException(ErrorCodes.DeckEmpty, "Plus de cartes disponibles");
                }

                _gameState.TransitionTo(GameStateEnum.WaitingForPlayerAction);
                OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));
            }
            catch (ErrorException e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        /// <summary>
        /// Démarre une nouvelle partie.
        /// </summary>
        public void StartGame()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;

            try
            {
                if (CurrentDeckCard == null && Deck.Cards.Count > 0)
                {
                    CurrentDeckCard = Deck.Cards.First();
                }

                if (CurrentDeckCard == null)
                {
                    throw new ErrorException(ErrorCodes.DeckEmpty, "Aucune carte disponible pour démarrer le jeu");
                }

                _gameState.TransitionTo(GameStateEnum.WaitingForPlayerAction);
                ProcessCardEffect(CurrentDeckCard, CurrentPlayer);
                OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));
            }
            catch (ErrorException e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void HandlePlayerChoice(Player player, string choice)
        {
            try
            {
                if (player != CurrentPlayer) throw new ErrorException(ErrorCodes.NotPlayerTurn);
                if (!_gameState.CanPerformAction(choice)) 
                    throw new ErrorException(ErrorCodes.InvalidAction, "Action non autorisée dans l'état actuel");

                switch (choice)
                {
                    case "1":
                        _gameState.TransitionTo(GameStateEnum.WaitingForCoverTarget);
                        OnPlayerChooseCover(new PlayerChooseCoverEventArgs(player));
                        break;
                    case "2":
                        _gameState.TransitionTo(GameStateEnum.WaitingForDuckTarget);
                        OnPlayerChooseDuck(new PlayerChooseDuckEventArgs(player));
                        break;
                    case "3":
                        OnPlayerChooseCoin(new PlayerChooseCoinEventArgs(player));
                        DoCoin(player);
                        _gameState.TransitionTo(GameStateEnum.ProcessingCardEffect);
                        ProcessTurn();
                        break;
                    case "4":
                        OnPlayerChooseShowPlayersGrid(new PlayerChooseShowPlayersGridEventArgs(Players));
                        break;
                    case "5":
                        OnPlayerChooseShowScores(new PlayerChooseShowScoresEventArgs(Players));
                        break;
                    case "6":
                        OnPlayerChooseQuit(new PlayerChooseQuitEventArgs(player, this));
                        if (CheckGameOverCondition())
                        {
                            _gameState.TransitionTo(GameStateEnum.GameOver);
                        }
                        break;
                    default:
                        throw new ErrorException(ErrorCodes.InvalidChoice);
                }
            }
            catch (ErrorException e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void HandlePlayerChooseCover(Player player, Position cardToMovePosition, Position cardToCoverPosition)
        {
            try
            {
                if (player != CurrentPlayer) throw new ErrorException(ErrorCodes.NotPlayerTurn);
                if (!_gameState.CanPerformAction("1"))
                    throw new ErrorException(ErrorCodes.InvalidAction, "Action Cover non autorisée dans l'état actuel");

                DoCover(player, cardToMovePosition, cardToCoverPosition);
                _gameState.TransitionTo(GameStateEnum.ProcessingCardEffect);
                ProcessTurn();
            }
            catch (ErrorException e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
                throw;
            }
        }

        public void HandlePlayerChooseDuck(Player player, Position cardToMovePosition, Position duckPosition)
        {
            try
            {
                if (player != CurrentPlayer) throw new ErrorException(ErrorCodes.NotPlayerTurn);
                if (!_gameState.CanPerformAction("2"))
                    throw new ErrorException(ErrorCodes.InvalidAction, "Action Duck non autorisée dans l'état actuel");

                DoDuck(player, cardToMovePosition, duckPosition);
                _gameState.TransitionTo(GameStateEnum.ProcessingCardEffect);
                ProcessTurn();
            }
            catch (ErrorException e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
                throw;
            }
        }

        public void TriggerGameOver()
        {
            IsFinished = true;
            LastGameFinishStatus = true;
            _gameState.TransitionTo(GameStateEnum.GameOver);
            OnGameIsOver(new GameIsOverEventArgs(true));
        }

        public static List<Position> GetValidDuckTargetPositions(Player forPlayer, Position cardToMovePosition, DeckCard currentDeckCard)
        {
            var validTargets = new HashSet<Position>();
            if (currentDeckCard == null || forPlayer == null) return validTargets.ToList();

            GameCard? cardBeingMoved = forPlayer.Grid.GetCard(cardToMovePosition);
            if (cardBeingMoved == null) return validTargets.ToList();

            if (forPlayer.Grid.GameCardsGrid.Count(c => c != null) == 1)
            {
                return validTargets.ToList();
            }

            var occupiedPositions = new HashSet<Position>(
                forPlayer.Grid.GameCardsGrid
                    .Where(c => c != null && c.Position != cardToMovePosition)
                    .Select(c => c.Position)
            );

            int[] dRow = { -1, 1, 0, 0 };
            int[] dCol = { 0, 0, -1, 1 };

            foreach (var card in forPlayer.Grid.GameCardsGrid)
            {
                if (card == null || card.Position == cardToMovePosition)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    var adjacent = new Position(card.Position.Row + dRow[i], card.Position.Column + dCol[i]);

                    if (occupiedPositions.Contains(adjacent))
                        continue;

                    validTargets.Add(adjacent);
                }
            }

            return validTargets.ToList();
        }

        public void DoCover(Player player, Position cardToMovePosition, Position cardToCoverPosition)
        {
            Rules.TryValidMove(cardToMovePosition, cardToCoverPosition, player.Grid, "cover", CurrentDeckCard);

            GameCard cardToMove = player.Grid.GetCard(cardToMovePosition)!;
            GameCard cardToCover = player.Grid.GetCard(cardToCoverPosition)!;
            List<GameCard> gridCards = player.Grid.GameCardsGrid;

            gridCards.Remove(cardToCover);
            cardToMove.Position = new Position(cardToCover.Position.Row, cardToCover.Position.Column);
            player.StackCounter = gridCards.Count;
            player.HasPlayed = true;

            NextPlayer();
        }

        public void DoDuck(Player player, Position cardToMovePosition, Position duckPosition)
        {
            Rules.TryValidMove(cardToMovePosition, duckPosition, player.Grid, "duck", CurrentDeckCard);

            GameCard cardToMove = player.Grid.GetCard(cardToMovePosition)!;

            player.Grid.RemoveCard(cardToMove.Position);
            cardToMove.Position = duckPosition;
            player.Grid.SetCard(duckPosition, cardToMove);

            player.StackCounter = player.Grid.GameCardsGrid.Count;
            player.HasPlayed = true;

            NextPlayer();
        }

        public void DoCoin(Player player)
        {
            Debug.WriteLine($"DoCoin - Player: {player.Name}, CardsSkipped before: {CardsSkipped}");
            player.StackCounter = player.Grid.GameCardsGrid.Count;
            player.HasPlayed = true;
            player.HasSkipped = true;

            if (Players.All(p => p.HasSkipped))
            {
                CardsSkipped++;
                Debug.WriteLine($"DoCoin - All players skipped, CardsSkipped after: {CardsSkipped}");
                Players.ForEach(p => p.HasSkipped = false);
            }

            NextPlayer();
        }

        public bool AllPlayersPlayed()
        {
            return Players.All(p => p.HasPlayed);
        }

        public void CheckAllPlayersSkipped()
        {
            if (Players.All(p => p.HasSkipped))
            {
                CardsSkipped++;
                Players.ForEach(p => p.HasSkipped = false);
            }
        }

        public void SavePlayers()
        {
            foreach (var player in Players)
            {
                if (!player.IsBot)
                {
                    int score = player.Grid.GameCardsGrid.Sum(card => card.Splash);
                    player.Scores.Add(score);
                    var existingPlayer = AllPlayers.FirstOrDefault(p => p.Name == player.Name);
                    if (existingPlayer != null)
                    {
                        existingPlayer.Scores.Add(score);
                    }
                    else
                    {
                        AllPlayers.Add(player);
                    }
                }
            }
        }

        public void SaveGame()
        {
            var existingGame = Games.FirstOrDefault(g => g.Id == Id);
            if (existingGame != null)
            {
                existingGame.IsFinished = true;
                existingGame.CardsSkipped = CardsSkipped;
                existingGame.LastGameFinishStatus = LastGameFinishStatus;
                existingGame.LastNumber = LastNumber;
                existingGame.Deck = Deck;
                existingGame.Players = Players;
                existingGame.Rules = Rules;
            }
            else
            {
                Games.Add(this);
            }
        }
    }
}