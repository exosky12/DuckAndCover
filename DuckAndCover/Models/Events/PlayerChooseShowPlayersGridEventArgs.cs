using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseShowPlayersGridEventArgs : EventArgs
    {
        public List<Player> Players { get; private set; }
        
        public PlayerChooseShowPlayersGridEventArgs(List<Player> players )
        {
            this.Players = players;
        }
    }
} 
