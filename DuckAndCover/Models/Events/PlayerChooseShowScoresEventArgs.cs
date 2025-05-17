using Models.Game;
    
namespace Models.Events
{
    public class PlayerChooseShowScoresEventArgs : EventArgs
    {
        public List<Player> Players { get; private set; }
        
        public PlayerChooseShowScoresEventArgs(List<Player> players )
        {
            this.Players = players;
        }
    }
} 
