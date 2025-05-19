using System.Diagnostics.CodeAnalysis;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class GameIsOverEventArgs
    {
        public readonly bool _gameIsOver;
        
        public GameIsOverEventArgs(bool gameIsOver)
        {
            this._gameIsOver = gameIsOver;
        }
    }
}