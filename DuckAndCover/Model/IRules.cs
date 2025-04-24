namespace Model;

public interface IRules
{
    string Name { get; }

    string Description { get; }

    int NbCardsInDeck { get; }

    bool IsValidMove(Position position, Position newPosition, Grid grid, string funcName);

    bool IsGameOver(int cardPassed, int stackCounter);
}