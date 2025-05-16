namespace Model;
using Exceptions;

public class Game
{
    private List<Player> Players { get; }

    public IRules Rules { get; }

    public int PlayerCount => Players.Count;

    public int CardPassed { get; set; }

    public Player CurrentPlayer { get; set; }

    public Deck Deck { get; } = new Deck();

    private DeckCard _currentDeckCard => Deck.Cards.FirstOrDefault();

    public int CurrentPlayerIndex { get; set; }

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
        this.CurrentPlayer = players[0];
        this.CurrentPlayerIndex = 0;
    }

    public void NextPlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        CurrentPlayer = Players[CurrentPlayerIndex];
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
            
            int aboveIndex = gameCardsGrid.FindIndex(c => c.Position.Row == cardToMove.Position.Row && c.Position.Column == cardToMove.Position.Column);
            int belowIndex = gameCardsGrid.FindIndex(c => c.Position.Row == cardToCover.Position.Row && c.Position.Column == cardToCover.Position.Column);

            if (aboveIndex >= 0 && belowIndex >= 0)
            {
                cardToMove.Position = new Position(cardToCover.Position.Row, cardToCover.Position.Column);
                gameCardsGrid[belowIndex] = cardToMove;
                gameCardsGrid.RemoveAt(aboveIndex > belowIndex ? aboveIndex + 1 : aboveIndex);
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
            throw new Error("Error while trying to duck card", e);
        }
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