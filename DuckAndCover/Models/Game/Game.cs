using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Models.Exceptions;
using Models.Interfaces;
using Models.Rules;
using Models.Events;
using Models.Enums;
using System.ComponentModel;

namespace Models.Game
{
    [DataContract]
    public class Game : INotifyPropertyChanged
    {
        [DataMember] public string Id { get; set; } = string.Empty;

        [DataMember] private ObservableCollection<Player> _allPlayers = new ObservableCollection<Player>();

        public ObservableCollection<Player> AllPlayers
        {
            get => _allPlayers;
            set
            {
                _allPlayers = value;
                OnPropertyChanged(nameof(AllPlayers));
            }
        }

        public List<Player> Players { get; set; } = new List<Player>();

        [DataMember] private ObservableCollection<Game> _games = new ObservableCollection<Game>();

        public ObservableCollection<Game> Games
        {
            get => _games;
            set
            {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        [DataMember]
        public IRules Rules { get; set; }

        [DataMember] public int CardsSkipped { get; set; }

        public Player CurrentPlayer { get; set; } = new Player("Default", 0, new List<int>(), false, false, new Grid());

        [DataMember] private int _currentPlayerIndex;

        [DataMember] public Deck Deck { get; set; } = new Deck();

        public bool Quit { get; set; }

        [DataMember] public bool IsFinished { get; set; }

        [DataMember] public bool LastGameFinishStatus { get; private set; }

        public DeckCard CurrentDeckCard { get; private set; } = new DeckCard(Bonus.None, 0);

        [DataMember] public int? LastNumber { get; set; }

        // État du jeu pour savoir s'il a démarré
        public bool IsGameStarted { get; private set; }

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

        public Game(IRules rules)
        {
            this.Rules = rules;
        }

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Nouvelle méthode pour démarrer le jeu
        public void StartGame()
        {
            if (IsGameStarted)
                return;

            IsGameStarted = true;
            
            try
            {
                // Déclencher l'événement pour le premier joueur
                OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));
            }
            catch (Error e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
            CurrentPlayer = Players[_currentPlayerIndex];
        }

        // Remplacer GameLoop par cette méthode qui sera appelée après chaque action
        public void ProcessTurn()
        {
            try
            {
                // Vérifier si tous les joueurs ont joué
                if (AllPlayersPlayed())
                {
                    NextDeckCard();
                    Players.ToList().ForEach(p =>
                    {
                        p.HasPlayed = false;
                        p.HasSkipped = false;
                    });
                }

                // Vérifier les conditions de fin de partie
                if (CheckGameOverCondition())
                {
                    return; // Le jeu est terminé
                }

                // Déclencher l'événement pour le joueur suivant
                OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));
            }
            catch (Error e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void HandlePlayerChoice(Player player, string choice)
        {
            try
            {
                switch (choice)
                {
                    case "1":
                        OnPlayerChooseCover(new PlayerChooseCoverEventArgs(CurrentPlayer));
                        break;
                    case "2":
                        OnPlayerChooseDuck(new PlayerChooseDuckEventArgs(CurrentPlayer));
                        break;
                    case "3":
                        OnPlayerChooseCoin(new PlayerChooseCoinEventArgs(CurrentPlayer));
                        DoCoin(player);
                        CheckAllPlayersSkipped();
                        ProcessTurn(); // Traiter le tour suivant
                        break;
                    case "4":
                        OnPlayerChooseShowPlayersGrid(new PlayerChooseShowPlayersGridEventArgs(Players));
                        OnDisplayMenuNeeded(new DisplayMenuNeededEventArgs(CurrentPlayer, CurrentDeckCard));
                        break;
                    case "5":
                        OnPlayerChooseShowScores(new PlayerChooseShowScoresEventArgs(Players));
                        OnDisplayMenuNeeded(new DisplayMenuNeededEventArgs(CurrentPlayer, CurrentDeckCard));
                        break;
                    case "6":
                        OnPlayerChooseQuit(new PlayerChooseQuitEventArgs(CurrentPlayer, this));
                        break;
                    default:
                        throw new Error(ErrorCodes.InvalidChoice);
                }
            }
            catch (Error e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void HandlePlayerChooseCover(Player player, Position cardToMovePosition, Position cardToCoverPosition)
        {
            try
            {
                DoCover(player, cardToMovePosition, cardToCoverPosition);
                ProcessTurn(); // Traiter le tour suivant après l'action
            }
            catch (Error e)
            {
                throw new Error(e.ErrorCode);
            }
        }

        public void TriggerGameOver()
        {
            OnGameIsOver(new GameIsOverEventArgs(true));
        }

        public void HandlePlayerChooseDuck(Player player, Position cardToMovePosition, Position duckPosition)
        {
            try
            {
                DoDuck(player, cardToMovePosition, duckPosition);
                ProcessTurn(); // Traiter le tour suivant après l'action
            }
            catch (Error e)
            {
                throw new Error(e.ErrorCode);
            }
        }

        public bool CheckGameOverCondition()
        {
            if (Rules.IsGameOver(CardsSkipped, CurrentPlayer.StackCounter, Quit))
            {
                IsFinished = true;
                OnGameIsOver(new GameIsOverEventArgs(true));
                return true;
            }

            return false;
        }

        public void DoCover(Player player, Position cardToMovePosition, Position cardToCoverPosition)
        {
            try
            {
                Rules.TryValidMove(cardToMovePosition, cardToCoverPosition, player.Grid, "cover", CurrentDeckCard);
                GameCard cardToMove = player.Grid.GetCard(cardToMovePosition)!;
                GameCard cardToCover = player.Grid.GetCard(cardToCoverPosition)!;
                List<GameCard> grid = player.Grid.GameCardsGrid;

                grid.Remove(cardToCover);
                cardToMove.Position = new Position(cardToCover.Position.Row, cardToCover.Position.Column);

                player.StackCounter = grid.Count;
                player.HasPlayed = true;

                NextPlayer();
            }
            catch (Error e)
            {
                throw new Error(e.ErrorCode);
            }
        }

        public void DoDuck(Player player, Position cardToMovePosition, Position duckPosition)
        {
            try
            {
                Rules.TryValidMove(cardToMovePosition, duckPosition, player.Grid, "duck", CurrentDeckCard);
                GameCard cardToMove = player.Grid.GetCard(cardToMovePosition)!;

                player.Grid.RemoveCard(cardToMove.Position);
                player.Grid.SetCard(duckPosition, cardToMove);

                player.StackCounter = player.Grid.GameCardsGrid.Count;
                player.HasPlayed = true;

                NextPlayer();
            }
            catch (Error e)
            {
                throw new Error(e.ErrorCode);
            }
        }

        public void DoCoin(Player player)
        {
            player.StackCounter = player.Grid.GameCardsGrid.Count;
            player.HasPlayed = true;
            player.HasSkipped = true;

            NextPlayer();
        }

        public DeckCard NextDeckCard()
        {
            if (Deck.Cards.Count == 0)
                throw new Error(ErrorCodes.DeckEmpty);

            Deck.Cards.RemoveAt(0);

            if (Deck.Cards.Count == 0)
                CurrentDeckCard = null!;
            else
                CurrentDeckCard = Deck.Cards.First();

            return CurrentDeckCard!;
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
            }
        }

        public void SavePlayers()
        {
            foreach (var player in Players)
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

        public void SaveGame()
        {
            var existingGame = Games.FirstOrDefault(g => g.Id == Id);
            if (existingGame != null)
            {
                existingGame.IsFinished = true;
            }
            else 
            {
                Games.Add(this);
            }
        }
    }
}