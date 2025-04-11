namespace Model
{



    public enum Bonus
    {
        max,
        bis
    }

    public class DeckCard : Card
    {
        public Bonus currentBonus { get; set; }

        public DeckCard(Bonus bonus, int num) : base(num)
        {
            currentBonus = bonus;
        }

        public void ApplyBonus(Card previousCard)
        {
            switch (currentBonus)
            {
                case Bonus.max:
                    //oblige le joueur a jouer sa carte la plus haute
                    break;
                case Bonus.bis:
                    //faire en sorte de recup la carte precedante pour faire un nouveau tour avec cette carte
                    break;
            }
        }
    }
}