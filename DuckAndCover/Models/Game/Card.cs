namespace Models.Game
{
    public abstract class Card
    {
        public int Number { get; set; }

        protected Card(int number)
        {
            this.Number = number;
        }
    }
}