using System.Diagnostics.CodeAnalysis;

namespace Models.Events

{
    [ExcludeFromCodeCoverage]
    public class GameIsOverEventArgs
    {
        private bool _gameIsOver;
        
        public GameIsOverEventArgs(bool gameIsOver)
        {
            this._gameIsOver = gameIsOver;
        }
    }
}