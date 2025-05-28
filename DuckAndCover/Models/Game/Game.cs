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
        [DataMember] public string Id { get; }

        [DataMember] private ObservableCollection<Player> _players;

        public ObservableCollection<Player> Players
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        [DataMember] private ObservableCollection<Game> _games;

        public ObservableCollection<Game> Games
        {
            get => _games;
            set
            {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }


        public IRules Rules { get; }

        [DataMember] public int CardsSkipped { get; set; }

        public Player CurrentPlayer { get; set; }

        [DataMember] private int _currentPlayerIndex;

        [DataMember] public Deck Deck { get; } = new Deck();

        public bool Quit { get; set; }

        [DataMember] public bool IsFinished { get; set; }

        [DataMember] public bool LastGameFinishStatus { get; private set; }

        public DeckCard CurrentDeckCard { get; private set; }

        [DataMember] public int? LastNumber { get; set; }

        public IDataPersistence DataManager { get; set; }


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


        public void LoadData()
        {
            var data = DataManager.LoadData();
            foreach (var player in data.Item1)
            {
                this.Players.Add(player);
            }

            foreach (var game in data.Item2)
            {
                if (game.IsFinished) LastGameFinishStatus = true;

                this.Games.Add(game);
            }
        }

        public void Save()
        {
            SaveScores();
            DataManager.SaveData(Players, Games);
        }

        public Game(IDataPersistence dataManager, ObservableCollection<Player>? players)
        {
            this.DataManager = dataManager;
            (ObservableCollection<Player> loadedPlayers, ObservableCollection<Game> loadedGames) = DataManager.LoadData();
            this._players = loadedPlayers;
            this._games = loadedGames;
            this.Id = Guid.NewGuid().ToString("N").Substring(0, 5);
            Players = new ObservableCollection<Player>();
            Games = new ObservableCollection<Game>();
            this.Quit = false;
            this.IsFinished = false;
            this.Players = players ?? new ObservableCollection<Player>();
            this.CurrentDeckCard = Deck.Cards.FirstOrDefault()!;
            this._currentPlayerIndex = 0;
            this.CurrentPlayer = Players.First();
            this.Rules = new ClassicRules();
        }

        public Game(string id, ObservableCollection<Player> players, int currentPlayerIndex, int cardsSkipped,
            bool isFinished, Deck deck, int? lastNumber)
        {
            this.Id = id;
            this.Players = players;
            this.Deck = deck;
            this._currentPlayerIndex = currentPlayerIndex;
            this.CurrentPlayer = players[_currentPlayerIndex];
            this.CurrentDeckCard = deck.Cards.FirstOrDefault()!; 
            this.CardsSkipped = cardsSkipped;
            this.IsFinished = isFinished;
            this.LastNumber = lastNumber;
            this.Rules = new ClassicRules();
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
            CurrentPlayer = Players[_currentPlayerIndex];
        }

        public void GameLoop()
        {
            bool isOver = false;

            while (!isOver)
            {
                try
                {
                    OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));

                    if (AllPlayersPlayed())
                    {
                        NextDeckCard();
                        Players.ToList().ForEach(p =>
                        {
                            p.HasPlayed = false;
                            p.HasSkipped = false;
                        });
                    }

                    isOver = CheckGameOverCondition();
                }
                catch (Error e)
                {
                    OnErrorOccurred(new ErrorOccurredEventArgs(e));
                }
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

        public void SaveScores()
        {
            foreach (var player in Players)
            {
                int score = player.Grid.GameCardsGrid.Sum(card => card.Splash);
                player.Scores.Add(score);
            }
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
    }
}