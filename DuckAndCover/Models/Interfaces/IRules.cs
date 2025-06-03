using Models.Game;

namespace Models.Interfaces
{
    public interface IRules
    {
        string Name { get; }

        string Description { get; }

        int NbCardsInDeck { get; }

        public void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard);

        bool IsGameOver(int cardPassed, int stackCounter, bool quit);

        bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard);
    }
}