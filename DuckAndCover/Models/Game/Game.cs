using Models.Exceptions;
using Models.Interfaces;
using Models.Rules;
using Models.Events;
using System.Threading;

namespace Models.Game
{
    public class Game
    {
        public List<Player> Players { get; }
        public IRules Rules { get; }
        public int PlayerCount => Players.Count;
        public int CardsSkipped { get; set; }
        public Player CurrentPlayer { get; set; }
        public Deck Deck { get; } = new Deck();
        public DeckCard CurrentDeckCard { get; set; }
        public int? LastNumber { get; set; }

        private int _currentPlayerIndex;
        private string? _pendingChoice;
        private readonly AutoResetEvent _choiceSubmitted = new AutoResetEvent(false);

        public event EventHandler<PlayerChangedEventArgs>? PlayerChanged;
        public event EventHandler<PlayerChooseEventArgs>? PlayerChoose;
        public event EventHandler<GameIsOverEventArgs>? GameIsOver;

        protected virtual void OnPlayerChanged(PlayerChangedEventArgs args) => PlayerChanged?.Invoke(this, args);
        protected virtual void OnPlayerChoose(PlayerChooseEventArgs args) => PlayerChoose?.Invoke(this, args);
        protected virtual void OnGameIsOver(GameIsOverEventArgs args) => GameIsOver?.Invoke(this, args);

        public Game(List<Player> players)
        {
            this.Rules = new ClassicRules();
            this.Players = players;
            this.CurrentDeckCard = Deck.Cards.FirstOrDefault()!;
            this._currentPlayerIndex = 0;
            this.CurrentPlayer = players[_currentPlayerIndex];
        }

        public void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
            CurrentPlayer = Players[_currentPlayerIndex];
        }

        public void SubmitChoice(string choice)
        {
            _pendingChoice = choice;
            _choiceSubmitted.Set();
        }

        public void GameLoop()
        {
            bool isOver = false;

            while (!isOver)
            {
                _pendingChoice = null;
                OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));

                _choiceSubmitted.WaitOne();
                HandlePlayerChoice(CurrentPlayer, _pendingChoice!);

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

                NextPlayer();
            }
        }


        public void HandlePlayerChoice(Player player, string choice)
        {
            OnPlayerChoose(new PlayerChooseEventArgs(choice));

            switch (choice)
            {
                case "1":
                    // À implémenter dans l'UI : demander les positions
                    break;
                case "2":
                    // À implémenter dans l'UI : demander les positions
                    break;
                case "3":
                    CallCoin(player);
                    CheckAllPlayersSkipped();
                    break;
                
                default:
                    break;
            }
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
            }
            catch (Error e)
            {
                throw new Error(e.ErrorCode);
            }
        }

        public void CallCoin(Player player)
        {
            player.StackCounter = player.Grid.GameCardsGrid.Count;
            player.HasPlayed = true;
            player.HasSkipped = true;
        }
        
        public DeckCard NextDeckCard()
        {
            if (Deck.Cards.Count == 0)
                throw new InvalidOperationException("No more cards in the deck.");

            Deck.Cards.RemoveAt(0);

            if (Deck.Cards.Count == 0)
                CurrentDeckCard = null!;
            else
                CurrentDeckCard = Deck.Cards.First();

            return CurrentDeckCard!;
        }


        public bool CheckGameOverCondition()
        {
            if (Rules.IsGameOver(CardsSkipped, CurrentPlayer.StackCounter))
            {
                OnGameIsOver(new GameIsOverEventArgs(true));
                return true;
            }

            return false;
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
