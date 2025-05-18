using Models.Game;
using System.Diagnostics.CodeAnalysis;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
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