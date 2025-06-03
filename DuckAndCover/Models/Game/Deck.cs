using Models.Enums;
using Models.Generators;
using System.Runtime.Serialization;

namespace Models.Game
{
    /// <summary>
    /// Représente un deck de cartes du jeu.
    /// </summary>
    [DataContract]
    public class Deck
    {
        /// <summary>
        /// Obtient ou définit la liste des cartes dans le deck.
        /// </summary>
        [DataMember]
        public List<DeckCard> Cards { get; set; } = new List<DeckCard>();

        /// <summary>
        /// Initialise une nouvelle instance de la classe Deck.
        /// Génère un nouveau deck de cartes et réorganise les cartes si nécessaire.
        /// </summary>
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