using Models.Game;
    
namespace Models.Events
{
    public class PlayerChooseCoverEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        
        public PlayerChooseCoverEventArgs(Player player)
        {
            this.Player = player;
        }
    }
} 
