using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Models.Exceptions;
using Models.Interfaces;
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

        public IRules Rules { get; set; }

        [DataMember] private string _rulesName = string.Empty;

        [DataMember] public int CardsSkipped { get; set; }

        public Player CurrentPlayer { get; set; } = new Player("Default", 0, new List<int>(), false, false, new Grid());

        [DataMember] private int _currentPlayerIndex;

        [DataMember] public Deck Deck { get; set; } = new Deck();

        public bool Quit { get; set; }

        [DataMember] public bool IsFinished { get; set; }

        [DataMember] public bool LastGameFinishStatus { get; private set; }

        public DeckCard CurrentDeckCard { get; private set; } = new DeckCard(Bonus.None, 0);

        [DataMember] public int? LastNumber { get; set; }

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
        public event EventHandler<CardEffectProcessedEventArgs>? CardEffectProcessed;

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
        protected virtual void OnCardEffectProcessed(CardEffectProcessedEventArgs args) => CardEffectProcessed?.Invoke(this, args);
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Game(IRules rules)
        {
            this.Rules = rules;
            this._rulesName = rules.GetType().Name;
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

        public void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
            CurrentPlayer = Players[_currentPlayerIndex];
        }

        public int GetEffectiveDeckCardNumber(Player forPlayer, DeckCard? deckCard)
        {
            if (deckCard == null)
            {
                throw new InvalidOperationException("Deck card cannot be null to get effective number.");
            }

            if (deckCard.Bonus == Bonus.Max)
            {
                if (forPlayer.Grid.GameCardsGrid.Any())
                {
                    return forPlayer.Grid.GameCardsGrid.Max(c => c.Number);
                }
                else
                {
                    return deckCard.Number;
                }
            }
            return deckCard.Number;
        }

        public void ProcessCardEffect(DeckCard card, Player playerProcessingEffect)
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
                    if (playerProcessingEffect.Grid.GameCardsGrid.Any())
                    {
                        maxEffectMessage = $"Carte MAX ! Le numéro sera le plus élevé de la grille du joueur actif.";
                    }
                    else
                    {
                        maxEffectMessage = $"Carte MAX ! Si la grille du joueur actif est vide, la carte prendra sa valeur de base ({card.Number}).";
                    }
                    OnCardEffectProcessed(new CardEffectProcessedEventArgs(maxEffectMessage, card));
                    break;

                default:
                    LastNumber = card.Number;
                    string msg = card.Number == 0 && card.Bonus != Bonus.None ? card.Bonus.ToString() : card.Number.ToString();
                    OnCardEffectProcessed(new CardEffectProcessedEventArgs(
                        $"Carte actuelle du deck : {msg}",
                        card));
                    break;
            }
        }
        
        public DeckCard NextDeckCard()
        {
            if (Deck.Cards.Count == 0)
                throw new Error(ErrorCodes.DeckEmpty);

            Deck.Cards.RemoveAt(0);

            if (Deck.Cards.Count == 0)
            {
                CurrentDeckCard = null!;
                throw new Error(ErrorCodes.DeckEmpty, "Plus de cartes dans le deck");
            }

            CurrentDeckCard = Deck.Cards.First();
            ProcessCardEffect(CurrentDeckCard, CurrentPlayer);

            return CurrentDeckCard;
        }

        public void ProcessTurn()
        {
            try
            {
                if (AllPlayersPlayed())
                {
                    NextDeckCard();
                    Players.ForEach(p =>
                    {
                        p.HasPlayed = false;
                        p.HasSkipped = false;
                    });
                }

                if (CheckGameOverCondition())
                {
                    return;
                }

                if (CurrentDeckCard == null)
                {
                    throw new Error(ErrorCodes.DeckEmpty, "Plus de cartes disponibles");
                }
                OnPlayerChanged(new PlayerChangedEventArgs(CurrentPlayer, CurrentDeckCard));
            }
            catch (Error e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void StartGame()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;

            try
            {
                if (CurrentDeckCard == null)
                {
                     throw new Error(ErrorCodes.DeckEmpty, "Aucune carte disponible pour démarrer le jeu");
                }
                ProcessCardEffect(CurrentDeckCard, CurrentPlayer);
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
                if (player != CurrentPlayer && choice != "4" && choice != "5" && choice != "6") {
                     OnErrorOccurred(new ErrorOccurredEventArgs(new Error(ErrorCodes.NotPlayerTurn, "Ce n'est pas votre tour.")));
                     return;
                }

                switch (choice)
                {
                    case "1":
                        OnPlayerChooseCover(new PlayerChooseCoverEventArgs(CurrentPlayer));
                        break;
                    case "2":
                        OnPlayerChooseDuck(new PlayerChooseDuckEventArgs(CurrentPlayer));
                        break;
                    case "3":
                        DoCoin(CurrentPlayer);
                        CheckAllPlayersSkipped();
                        OnPlayerChooseCoin(new PlayerChooseCoinEventArgs(CurrentPlayer));
                        ProcessTurn();
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
                        if (this.Quit) {
                             CheckGameOverCondition();
                        }
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
                if (player != CurrentPlayer) throw new Error(ErrorCodes.NotPlayerTurn);
                DoCover(player, cardToMovePosition, cardToCoverPosition);
                ProcessTurn();
            }
            catch (Error e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }

        public void TriggerGameOver()
        {
            IsFinished = true;
            LastGameFinishStatus = true;
            OnGameIsOver(new GameIsOverEventArgs(true));
        }

        public void HandlePlayerChooseDuck(Player player, Position cardToMovePosition, Position duckPosition)
        {
            try
            {
                if (player != CurrentPlayer) throw new Error(ErrorCodes.NotPlayerTurn);
                DoDuck(player, cardToMovePosition, duckPosition);
                ProcessTurn();
            }
            catch (Error e)
            {
                OnErrorOccurred(new ErrorOccurredEventArgs(e));
            }
        }
        
        public bool CheckGameOverCondition()
        {
            if (Rules.IsGameOver(CardsSkipped, CurrentPlayer.StackCounter, Quit))
            {
                TriggerGameOver();
                return true;
            }
            return false;
        }

        public void DoCover(Player player, Position cardToMovePosition, Position cardToCoverPosition)
        {
            int effectiveDeckCardNumber = GetEffectiveDeckCardNumber(player, CurrentDeckCard);
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

        public void DoDuck(Player player, Position cardToMovePosition, Position duckPosition)
        {
            int effectiveDeckCardNumber = GetEffectiveDeckCardNumber(player, CurrentDeckCard);
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
            player.StackCounter = player.Grid.GameCardsGrid.Count;
            player.HasPlayed = true;
            player.HasSkipped = true;

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
                int score = player.Grid.GameCardsGrid.Sum(card => card.Splash);
                var existingPlayer = AllPlayers.FirstOrDefault(p => p.Name == player.Name);
                if (existingPlayer != null)
                {
                     if (!existingPlayer.Scores.Contains(score) || existingPlayer.Scores.LastOrDefault() != score)
                     {
                          existingPlayer.Scores.Add(score);
                     }
                }
                else
                {
                    player.Scores.Add(score);
                    AllPlayers.Add(player);
                }
            }
        }

        public void SaveGame()
        {
            var existingGame = Games.FirstOrDefault(g => g.Id == Id);
            if (existingGame != null)
            {
                existingGame.IsFinished = this.IsFinished;
                existingGame.LastGameFinishStatus = this.LastGameFinishStatus;
            }
            else
            {
                Games.Add(this);
            }
        }
    }
}