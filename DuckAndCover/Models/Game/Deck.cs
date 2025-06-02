using Models.Enums;
using Models.Generators;
using System.Runtime.Serialization;


namespace Models.Game
{
    [DataContract]
    public class Deck
    {
        [DataMember]
        public List<DeckCard> Cards { get; set; } = new();

        public Deck()
        {
            var generator = new DeckGenerator();
            Cards = new List<DeckCard>(generator.Generate());
            if (Cards[0].Bonus == Bonus.Again)
            {
                DeckCard tmp = Cards[0];
                Cards.RemoveAt(0);
                Cards.Add(tmp);
            }
        }
    }
}