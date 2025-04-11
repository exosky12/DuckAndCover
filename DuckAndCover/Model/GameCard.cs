namespace Model
{
    public class GameCard : Card
    {
        public int Task { get; set; }

        public GameCard(int task, int num) : base(num)
        {
            Task = task;
        }
    }
}