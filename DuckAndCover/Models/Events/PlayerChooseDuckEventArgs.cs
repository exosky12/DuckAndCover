using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour le choix de canard par un joueur.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlayerChooseDuckEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient le joueur qui a fait le choix.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerChooseDuckEventArgs.
        /// </summary>
        /// <param name="player">Le joueur qui a fait le choix.</param>
        public PlayerChooseDuckEventArgs(Player player)
        {
            Player = player;
        }
    }
} 
