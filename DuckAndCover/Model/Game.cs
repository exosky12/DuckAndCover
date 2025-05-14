namespace Model;

public class Game
{
    private List<Player> Players { get; }
    
    public IRules Rules { get; }

    public int PlayerCount => Players.Count;

    public int CardPassed { get; set; }

    public Player CurrentPlayer { get; set; }

    public Deck Deck { get; } = new Deck();

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

    public void Save()
    {
        foreach (var player in Players)
        {
            int score = 0;
            foreach(var card in player.Grid.GameCardsGrid)
            {
                score += card.Splash;
            }
            player.Scores.Add(score);
        }
    }
}