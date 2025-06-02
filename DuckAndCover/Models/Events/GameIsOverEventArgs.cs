using System.Diagnostics.CodeAnalysis;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class GameIsOverEventArgs : EventArgs
    {
        public readonly bool _gameIsOver;
        
        public GameIsOverEventArgs(bool gameIsOver)
        {
            this._gameIsOver = gameIsOver;
        }
    }
}