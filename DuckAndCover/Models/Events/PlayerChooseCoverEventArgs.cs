using Models.Game;
using System.Diagnostics.CodeAnalysis;
    
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseCoverEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        
        public PlayerChooseCoverEventArgs(Player player)
        {
            this.Player = player;
        }
    }
} 
