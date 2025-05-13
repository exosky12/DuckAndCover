namespace Model;

public class GameCard : Card
{
    public Position Position;
    public int Splash { get; set; }

    public GameCard(int splash, int number) : base(number)
    {
        this.Position = new Position(0, 0);
        this.Splash = splash;
    }
}