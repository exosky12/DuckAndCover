using Models.Enums;
using System.Runtime.Serialization;

namespace Models.Game
{
    [DataContract]
    public class DeckCard : Card
    {
        [DataMember]
        public Bonus Bonus { get; private set; }

        public DeckCard(int number) : base(number)
        {
            this.Bonus = Bonus.None;
        }

        public DeckCard(Bonus bonus, int number) : base(number)
        {
            this.Bonus = bonus;
        }
    }
}