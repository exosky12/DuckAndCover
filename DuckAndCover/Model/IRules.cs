namespace Model;

public interface IRules
{
    string Name { get; }

    string Description { get; }

    int NbCardsInDeck { get; }

    void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard);

    bool IsGameOver(int cardPassed, int stackCounter);

    bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard);
}