using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class DisplayMenuNeededEventArgs : EventArgs
    {
        public Player CurrentPlayer { get; }
        public DeckCard DeckCard { get; }

        public DisplayMenuNeededEventArgs(Player currentPlayer, DeckCard deckCard)
        {
            CurrentPlayer = currentPlayer;
            DeckCard = deckCard;
        }
    }
}