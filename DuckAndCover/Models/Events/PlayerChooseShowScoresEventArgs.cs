using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseShowScoresEventArgs : EventArgs
    {
        public ObservableCollection<Player> Players { get; private set; }
        
        public PlayerChooseShowScoresEventArgs(ObservableCollection<Player> players )
        {
            this.Players = players;
        }
    }
} 