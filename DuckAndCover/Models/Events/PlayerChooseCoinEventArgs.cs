using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour le choix de pièce par un joueur.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlayerChooseCoinEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient le joueur qui a fait le choix.
        /// </summary>
        public Player Player { get; }
        
        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerChooseCoinEventArgs.
        /// </summary>
        /// <param name="player">Le joueur qui a fait le choix.</param>
        public PlayerChooseCoinEventArgs(Player player)
        {
            Player = player;
        }
    }
} 
