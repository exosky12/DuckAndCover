namespace Models.Events
{
    public class PlayerChooseEventArgs : EventArgs
    {
        private readonly string _choice;
        
        public PlayerChooseEventArgs(string choice)
        {
            this._choice = choice;
        }
    }
}