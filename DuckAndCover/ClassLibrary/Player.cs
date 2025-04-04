namespace ClassLibrary
{
    public class Player
    {
        public Grid Grid { get; set; }
        public string Name { get; set; }
        public int GameScore { get; set; }
        public int TotalScore { get; set; }

        public Player(string name)
        {
            Name = name;
            GameScore = 0;
            TotalScore = 0;
        }

        public void Cover(Grid grid)
        {
            /* TODO */
        }

        public void Move(Grid grid)
        {
            /* TODO */
        }

        public void MakeCardDisappear()
        {
            /* TODO */
        }

        public void CallCannotPlay()
        {
            /* TODO */
        }

        public void CallCoin()
        {
            /* TODO */
        }
    }
}