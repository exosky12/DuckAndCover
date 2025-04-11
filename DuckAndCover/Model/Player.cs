namespace Model
{
    public class Player
    {
        public string Name { get; }
        public int GameScore { get; set; }
        public Grid Grid { get; set; }
        public int TotalScore { get; set; }

        public Player(string name)
        {
            Name = name;
            GameScore = 0;
            Grid = new Grid(5, 5);
        }

        public void ResetGameScore()
        {
            GameScore = 0;
        }

        public void Cover(Position position, Grid grid)
        {
            /* TODO */
        }

        public void MoveTo(Card card, Position position, Grid grid)
        {
            /* TODO */
        }

        public void MakeCardDisappear(Card card, Position position, Grid grid)
        {
            /* TODO */
        }

        public void CallCoin(Game game)
        {
            game.SkipNextPlayer();
        }
    }
}