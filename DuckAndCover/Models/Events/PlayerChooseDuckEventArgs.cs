using Models.Game;
using System.Diagnostics.CodeAnalysis;
namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class PlayerChooseDuckEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        
        public PlayerChooseDuckEventArgs(Player player)
        {
            this.Player = player;
        }
    }
} 
