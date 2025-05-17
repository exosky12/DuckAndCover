using Models.Game;
    
namespace Models.Events
{
    public class PlayerChooseDuckEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        
        public PlayerChooseDuckEventArgs(Player player)
        {
            this.Player = player;
        }
    }
} 
