using Models.Exceptions;
using Models.Interfaces;
using Models.Rules;

namespace Models.Game
{
    public class Game
    {
        private List<Player> Players { get; }

        public IRules Rules { get; }

        public int PlayerCount => Players.Count;

        public int CardPassed { get; set; }

        public Player CurrentPlayer { get; set; }

        public Deck Deck { get; } = new Deck();

        private DeckCard _currentDeckCard;

        private int _currentPlayerIndex;

        public int? LastNumber { get; set; }

        public event Action? OnGameOver;

        public void CheckGameOverCondition()
        {
            if (Rules.IsGameOver(CardPassed, CurrentPlayer.StackCounter))
            {
                OnGameOver?.Invoke();
            }
        }

        public event Action<Player>? OnPlayerChanged;

        public void NotifyPlayerChanged()
        {
            OnPlayerChanged?.Invoke(CurrentPlayer);
        }

        public Game(List<Player> players)
        {
            this.Rules = new ClassicRules();
            this.Players = players;
            this._currentDeckCard = Deck.Cards.FirstOrDefault()!;
        }

        public void NextPlayer()
        {
            if (CurrentPlayer == null)
            {
                _currentPlayerIndex = 0;
                CurrentPlayer = Players[_currentPlayerIndex];
                OnPlayerChanged?.Invoke(CurrentPlayer);
            }

            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
            CurrentPlayer = Players[_currentPlayerIndex];
            OnPlayerChanged?.Invoke(CurrentPlayer);
        }

        public void DoCover(Player player, Position cardToMovePosition, Position cardToCoverPosition)
        {
            try
            {
                Rules.TryValidMove(cardToMovePosition, cardToCoverPosition, player.Grid, "cover", _currentDeckCard);
                GameCard cardToMove = player.Grid.GetCard(cardToMovePosition)!;
                GameCard cardToCover = player.Grid.GetCard(cardToCoverPosition)!;
                List<GameCard> gameCardsGrid = player.Grid.GameCardsGrid;

                int cardToMoveIndex = gameCardsGrid.FindIndex(c =>
                    c.Position.Row == cardToMove.Position.Row && c.Position.Column == cardToMove.Position.Column);
                int cardToCoverIndex = gameCardsGrid.FindIndex(c =>
                    c.Position.Row == cardToCover.Position.Row && c.Position.Column == cardToCover.Position.Column);

                if (cardToMoveIndex >= 0 && cardToCoverIndex >= 0)
                {
                    cardToMove.Position = new Position(cardToCover.Position.Row, cardToCover.Position.Column);
                    gameCardsGrid.Remove(cardToCover);
                }

                NextPlayer();
                player.StackCounter = gameCardsGrid.Count;
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
                Rules.TryValidMove(cardToMovePosition, duckPosition, player.Grid, "duck", _currentDeckCard);
                GameCard cardToMove = player.Grid.GetCard(cardToMovePosition)!;
                player.Grid.RemoveCard(cardToMove.Position);
                player.Grid.SetCard(duckPosition, cardToMove);
                NextPlayer();
                player.StackCounter = player.Grid.GameCardsGrid.Count;
            }
            catch (Error e)
            {
                throw new Error(e.ErrorCode);
            }
        }

        public void CallCoin(Player player)
        {
            NextPlayer();
            player.StackCounter = player.Grid.GameCardsGrid.Count;
            /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer donc on dit "coin" */
        }

        public void GameLoop()
        {
            bool exitGame = false;
            while (!exitGame)
            {
                CheckGameOverCondition();
                NextPlayer();
            }
        }

        public void HandlePlayerChoice()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            foreach (var player in Players)
            {
                int score = 0;
                foreach (var card in player.Grid.GameCardsGrid)
                {
                    score += card.Splash;
                }

                player.Scores.Add(score);
            }
        }
    }
}