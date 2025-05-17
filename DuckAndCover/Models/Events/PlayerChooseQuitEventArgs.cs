using Models.Game;
using GameModel = Models.Game.Game;
    
namespace Models.Events
{
    public class PlayerChooseQuitEventArgs : EventArgs
    {
        public Player Player { get; private set; }
        public GameModel CurrentGame { get; private set; }
        
        public PlayerChooseQuitEventArgs(Player player, GameModel game)
        {
            this.Player = player;
            this.CurrentGame = game;
        }
    }
} 
