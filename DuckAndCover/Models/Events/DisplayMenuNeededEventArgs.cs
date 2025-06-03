using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour l'affichage du menu.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DisplayMenuNeededEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient le joueur actuel.
        /// </summary>
        public Player CurrentPlayer { get; }

        /// <summary>
        /// Obtient la carte actuelle du deck.
        /// </summary>
        public DeckCard CurrentDeckCard { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe DisplayMenuNeededEventArgs.
        /// </summary>
        /// <param name="currentPlayer">Le joueur actuel.</param>
        /// <param name="currentDeckCard">La carte actuelle du deck.</param>
        public DisplayMenuNeededEventArgs(Player currentPlayer, DeckCard currentDeckCard)
        {
            CurrentPlayer = currentPlayer;
            CurrentDeckCard = currentDeckCard;
        }
    }
}