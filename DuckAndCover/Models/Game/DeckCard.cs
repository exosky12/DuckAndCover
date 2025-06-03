using Models.Enums;
using System.Runtime.Serialization;

namespace Models.Game
{
    /// <summary>
    /// Représente une carte du deck avec un bonus potentiel.
    /// </summary>
    [DataContract]
    public class DeckCard : Card
    {
        /// <summary>
        /// Obtient le bonus associé à la carte.
        /// </summary>
        [DataMember]
        public Bonus Bonus { get; private set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe DeckCard sans bonus.
        /// </summary>
        /// <param name="number">Le numéro de la carte.</param>
        public DeckCard(int number) : base(number)
        {
            this.Bonus = Bonus.None;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe DeckCard avec un bonus.
        /// </summary>
        /// <param name="bonus">Le bonus à associer à la carte.</param>
        /// <param name="number">Le numéro de la carte.</param>
        public DeckCard(Bonus bonus, int number) : base(number)
        {
            this.Bonus = bonus;
        }
    }
}