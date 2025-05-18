namespace Models.Events
using System.Diagnostics.CodeAnalysis;
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