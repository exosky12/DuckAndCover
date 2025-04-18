namespace Model;

public class Game
{
    public List<Player> players;

    public int PlayerCount => players.Count;

    public int CardPassed { get; set; } = 0;

    public Player CurrentPlayer => players[0];

    public Deck Deck { get; set; } = new Deck();

    public void SaveScores()
    {
        /* TODO */
    }

    public Game(List<Player> players)
    {
        this.players = players;
    }
}