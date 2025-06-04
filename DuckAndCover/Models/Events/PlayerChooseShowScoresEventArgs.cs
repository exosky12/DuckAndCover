using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour l'affichage des scores des joueurs.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlayerChooseShowScoresEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient la liste des joueurs dont les scores doivent être affichés.
        /// </summary>
        public List<Player> Players { get; }
        
        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerChooseShowScoresEventArgs.
        /// </summary>
        /// <param name="players">La liste des joueurs dont les scores doivent être affichés.</param>
        public PlayerChooseShowScoresEventArgs(List<Player> players)
        {
            Players = players;
        }
    }
} 