using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseShowPlayersGridEventArgs : EventArgs
    {
        public ObservableCollection<Player> Players { get; private set; }
        
        public PlayerChooseShowPlayersGridEventArgs(ObservableCollection<Player> players )
        {
            this.Players = players;
        }
    }
} 
