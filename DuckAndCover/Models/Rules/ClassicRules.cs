using Models.Exceptions;
using Models.Interfaces;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    public class ClassicRules : IRules
    {
        public string Name => "Règles Classiques";

        public string Description =>
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris";

        public int NbCardsInDeck => 24;

        public void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard)
        {
            GameCard? card = grid.GetCard(position);
            if (card == null)
                throw new Error(ErrorCodes.CardNotFound);

            if (card.Number != currentDeckCard.Number)
                throw new Error(ErrorCodes.CardNumberNotEqualToDeckCardNumber);

            switch (funcName.ToLower())
            {
                case "duck":
                    GameCard? cardToDuck = grid.GetCard(newPosition);
                    if (cardToDuck != null)
                        throw new Error(ErrorCodes.CardAlreadyExists);
                    
                    (bool isAdjacent, GameCard? adjacentCard) = grid.IsAdjacentToCard(newPosition);
                    
                    if (!isAdjacent || Equals(adjacentCard?.Position, position))
                        throw new Error(ErrorCodes.CardsAreNotAdjacent);

                    return;

                case "cover":
                    GameCard? cardToCover = grid.GetCard(newPosition);
                    if (cardToCover == null)
                        throw new Error(ErrorCodes.CardNotFound);
                    if (!grid.AreAdjacentCards(position, newPosition))
                        throw new Error(ErrorCodes.CardsAreNotAdjacent);
                    return;

                default:
                    throw new Error(ErrorCodes.InvalidFunctionName);
            }
        }

        public bool IsGameOver(int cardPassed, int stackCounter, bool quit) => cardPassed == 8 || stackCounter == 1 || quit;

        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }
    }
}