using System.Diagnostics.CodeAnalysis;
using Models.Game;
    
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseCoinEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        
        public PlayerChooseCoinEventArgs(Player player)
        {
            this.Player = player;
        }
    }
} 
