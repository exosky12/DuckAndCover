using System.Runtime.Serialization;

namespace Models.Game
{
    /// <summary>
    /// Représente une carte de jeu avec sa position et son effet de splash.
    /// </summary>
    [DataContract]
    public class GameCard : Card
    {
        /// <summary>
        /// Obtient ou définit la position de la carte sur le plateau.
        /// </summary>
        [DataMember]
        public Position Position { get; set; }
        
        /// <summary>
        /// Obtient ou définit la valeur de splash de la carte.
        /// </summary>
        [DataMember]
        public int Splash { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe GameCard.
        /// </summary>
        /// <param name="splash">La valeur de splash de la carte.</param>
        /// <param name="number">Le numéro de la carte.</param>
        public GameCard(int splash, int number) : base(number)
        {
            this.Position = new Position(0, 0);
            this.Splash = splash;
        }
    }
}