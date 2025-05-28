using Models.Exceptions;
using Models.Interfaces;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    public class InsaneRules : IRules
    {
        public string Name => "Règles XXL";

        public int NbCardsInDeck => 128;

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

                    if (grid.IsAdjacentToCard(newPosition) == (false, null))
                        throw new Error(ErrorCodes.AdjacentCardNotFound);
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

        public bool IsGameOver(int cardPassed, int stackCounter, bool quit) => cardPassed == 16 || stackCounter == 7 || quit;

        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }


    }
}
