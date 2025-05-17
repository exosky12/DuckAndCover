namespace Models.Events
{
    public class GameIsOverEventArgs
    {
        private bool _gameIsOver;
        
        public GameIsOverEventArgs(bool gameIsOver)
        {
            this._gameIsOver = gameIsOver;
        }
    }
}