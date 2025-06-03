using System.Runtime.Serialization;

namespace Models.Game
{
    /// <summary>
    /// Classe abstraite représentant une carte dans le jeu. Peut aussi bien être une carte de deck ou une carte de jeu.
    /// </summary>
    [DataContract]
    public abstract class Card
    {
        /// <summary>
        /// Obtient ou définit le numéro de la carte.
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Card.
        /// </summary>
        /// <param name="number">Le numéro de la carte.</param>
        protected Card(int number)
        {
            this.Number = number;
        }
    }
}