using Models.Exceptions;
using Models.Interfaces;
using Models.Rules;
using Models.Events;
using Models.Enums;

namespace Models.Game
{
    public class Game
    {
        public List<Player> Players { get; }
        public IRules Rules { get; }
        public int CardsSkipped { get; set; }
        public Player CurrentPlayer { get; set; }
        public Deck Deck { get; } = new Deck();
        public bool Quit { get; set; }
        public bool IsFinished { get; set; }  
        public DeckCard CurrentDeckCard { get; set; }
        public int? LastNumber { get; set; }

        private readonly string _id;
        public string Id { get; }

        public static event EventHandler<GameStartedEventArgs>? GameStarted;
        public static event EventHandler<GameResumedEventArgs>? GameResumed;

         public static void RaiseGameStarted(Game game) => GameStarted?.Invoke(null, new GameStartedEventArgs(game));
        public static void RaiseGameResumed(Game game) => GameResumed?.Invoke(null, new GameResumedEventArgs(game));


        private int _currentPlayerIndex;
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
        protected virtual void OnPlayerChanged(PlayerChangedEventArgs args) => PlayerChanged?.Invoke(this, args);
        protected virtual void OnErrorOccurred(ErrorOccurredEventArgs args) => ErrorOccurred?.Invoke(this, args);
        protected virtual void OnGameIsOver(GameIsOverEventArgs args) => GameIsOver?.Invoke(this, args);
        protected virtual void OnPlayerChooseCoin(PlayerChooseCoinEventArgs args) => PlayerChooseCoin?.Invoke(this, args);
        protected virtual void OnPlayerChooseDuck(PlayerChooseDuckEventArgs args) => PlayerChooseDuck?.Invoke(this, args);
        protected virtual void OnDisplayMenuNeeded(DisplayMenuNeededEventArgs args) => DisplayMenuNeeded?.Invoke(this, args);
        protected virtual void OnPlayerChooseQuit(PlayerChooseQuitEventArgs args) => PlayerChooseQuit?.Invoke(this, args);
        protected virtual void OnPlayerChooseShowScores(PlayerChooseShowScoresEventArgs args) => PlayerChooseShowScores?.Invoke(this, args);
        protected virtual void OnPlayerChooseShowPlayersGrid(PlayerChooseShowPlayersGridEventArgs args) => PlayerChooseShowPlayersGrid?.Invoke(this, args);
        protected virtual void OnPlayerChooseCover(PlayerChooseCoverEventArgs args) => PlayerChooseCover?.Invoke(this, args);

        public Game(List<Player> players)
        {
            this.Rules = new ClassicRules();
            this._id = Guid.NewGuid().ToString("N").Substring(0, 5);
            this.Id = _id;
            this.Quit = false;
            this.IsFinished = false;
            this.Players = players;
            this.CurrentDeckCard = Deck.Cards.FirstOrDefault()!;
            this._currentPlayerIndex = 0;
            this.CurrentPlayer = players[_currentPlayerIndex];
        }

        public Game(string id, List<Player> players, int currentPlayerIndex, int cardsSkipped, bool isFinished)
            : this(players)
        {
            this._id = id;
            this.Id = _id;
            this.Rules = new ClassicRules();
            this.Players = players;
            this._currentPlayerIndex = currentPlayerIndex;
            this.CurrentPlayer = players[_currentPlayerIndex];
            this.CurrentDeckCard = Deck.Cards.FirstOrDefault()!;
            this.CardsSkipped = cardsSkipped;
            this.IsFinished = isFinished;
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
                        Players.ForEach(p =>
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

        public void Save()
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
