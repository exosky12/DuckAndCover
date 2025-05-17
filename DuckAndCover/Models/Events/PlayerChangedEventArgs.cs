using Models.Game;

namespace Models.Events
{
    public class PlayerChangedEventArgs : EventArgs
    {
        public Player CurrentPlayer { get; }
        public DeckCard DeckCard { get; }

        public PlayerChangedEventArgs(Player currentPlayer, DeckCard deckCard)
        {
            CurrentPlayer = currentPlayer;
            DeckCard = deckCard;
        }
    }
}