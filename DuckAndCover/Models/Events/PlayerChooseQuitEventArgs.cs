using System.Diagnostics.CodeAnalysis;
using Models.Game;
using GameModel = Models.Game.Game;
    
namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour le choix de quitter par un joueur.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlayerChooseQuitEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient le joueur qui a fait le choix.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Obtient la partie en cours.
        /// </summary>
        public GameModel CurrentGame { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerChooseQuitEventArgs.
        /// </summary>
        /// <param name="player">Le joueur qui a fait le choix.</param>
        /// <param name="game">La partie en cours.</param>
        public PlayerChooseQuitEventArgs(Player player, GameModel game)
        {
            Player = player;
            CurrentGame = game;
        }
    }
} 
