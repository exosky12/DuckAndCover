namespace Model;

public class Game
{
    public List<Player> Players;

    public IRules Rules { get; private set; }

    public int PlayerCount => Players.Count;

    public int CardPassed { get; set; } = 0;

    public Player CurrentPlayer { get; set; }

    public Deck Deck { get; set; } = new Deck();

    private int _currentPlayerIndex = 0;

    public void SaveScores()
    {
        /* TODO */
    }

    public Game(List<Player> players)
    {
        this.Rules = new ClassicRules();
        this.Players = players;
        this.CurrentPlayer = players[0];
        this._currentPlayerIndex = 0;
    }

    public void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        CurrentPlayer = Players[_currentPlayerIndex];
    }
}