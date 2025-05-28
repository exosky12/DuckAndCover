using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseShowScoresEventArgs : EventArgs
    {
        public List<Player> Players { get; private set; }
        
        public PlayerChooseShowScoresEventArgs(List<Player> players )
        {
            this.Players = players;
        }
    }
} 