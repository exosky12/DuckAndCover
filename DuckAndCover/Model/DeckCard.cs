namespace Model
{
    public class DeckCard : Card
    {
        public string Bonus { get; set; }

        public DeckCard(string bonus) : base(0)
        {
            Bonus = bonus;
        }

        public void ApplyBonus()
        {
            /* TODO */
        }
    }
}