using Models.Game;
    
namespace Models.Events
{
    public class PlayerChooseCoinEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        
        public PlayerChooseCoinEventArgs(Player player)
        {
            this.Player = player;
        }
    }
} 
