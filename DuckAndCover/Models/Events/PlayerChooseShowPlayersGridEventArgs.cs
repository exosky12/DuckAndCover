using Models.Game;
    
namespace Models.Events
{
    public class PlayerChooseShowPlayersGridEventArgs : EventArgs
    {
        public List<Player> Players { get; private set; }
        
        public PlayerChooseShowPlayersGridEventArgs(List<Player> players )
        {
            this.Players = players;
        }
    }
} 
