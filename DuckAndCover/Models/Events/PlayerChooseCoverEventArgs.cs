using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour le choix de couverture par un joueur.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlayerChooseCoverEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient le joueur qui a fait le choix.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerChooseCoverEventArgs.
        /// </summary>
        /// <param name="player">Le joueur qui a fait le choix.</param>
        public PlayerChooseCoverEventArgs(Player player)
        {
            Player = player;
        }
    }
} 
