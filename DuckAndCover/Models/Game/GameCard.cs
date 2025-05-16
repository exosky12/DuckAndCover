namespace Models.Game
{
    public class GameCard : Card
    {
        public Position Position { get; set; }
        public int Splash { get; }

        public GameCard(int splash, int number) : base(number)
        {
            this.Position = new Position(0, 0);
            this.Splash = splash;
        }
    }
}