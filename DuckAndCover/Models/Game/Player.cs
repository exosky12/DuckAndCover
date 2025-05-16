namespace Models.Game
{
    public class Player
    {
        public string Name { get; init; }
        public bool HasPassed { get; set; }
        public bool HasPlayed { get; set; }

        public List<int> Scores { get; }

        public int TotalScore => Scores.Sum();

        public int StackCounter { get; set; }

        public Player(string name)
        {
            this.StackCounter = 0;
            this.Scores = new List<int>();
            this.Name = name;
            this.HasPassed = false;
            this.HasPlayed = false;
        }

        public Grid Grid { get; } = new Grid();


        public bool HasCardWithNumber(int number)
        {
            foreach (var card in Grid.GameCardsGrid)
            {
                if (card.Number == number)
                    return true;
            }

            return false;
        }
    }
}