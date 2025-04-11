namespace Model
{
    public class Game
    {
        public Player Player { get; set; }
        public Deck Deck { get; set; }
        public int PlayerCount { get; set; }
        public List<Card> FinishCondition { get; set; }
        public int Turn { get; set; }

        public Game()
        {
            FinishCondition = new List<Card>();
            Turn = 0;
        }

        public Player SkipNextPlayer()
        {
            /* TODO */
            return null;
        }

        public int GetTurn() => Turn;

        public void SaveScore()
        {
            /* TODO */
        }
    }
}