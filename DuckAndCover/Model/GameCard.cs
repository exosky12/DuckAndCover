namespace Model
{
    public class GameCard : Card
    {
        public int Task { get; set; }

        public GameCard(int task) : base(task)
        {
            Task = task;
        }
    }
}