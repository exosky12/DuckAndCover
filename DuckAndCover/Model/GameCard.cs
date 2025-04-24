namespace Model;

public class GameCard : Card
{
    public Position position;
    public int Splash { get; private set; }

    public GameCard(int splash, int number) : base(number)
    {
        this.position = new Position(0, 0);
        this.Splash = splash;
    }
}