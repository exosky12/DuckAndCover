using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour l'affichage des grilles des joueurs.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlayerChooseShowPlayersGridEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient la liste des joueurs dont les grilles doivent être affichées.
        /// </summary>
        public List<Player> Players { get; }
        
        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerChooseShowPlayersGridEventArgs.
        /// </summary>
        /// <param name="players">La liste des joueurs dont les grilles doivent être affichées.</param>
        public PlayerChooseShowPlayersGridEventArgs(List<Player> players)
        {
            Players = players;
        }
    }
} 
