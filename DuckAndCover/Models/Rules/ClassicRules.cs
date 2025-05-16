using Models.Exceptions;
using Models.Interfaces;
using Models.Game;

namespace Models.Rules
{
    public class ClassicRules : IRules
    {
        public string Name => "Règles Classiques";

        public string Description =>
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris";

        public int NbCardsInDeck => 24;


        /* Check si y'a une carte à l'endroit de position --> si non alors retourner false, si oui --> regarder la méthode qui est appelé
         grâce à funcName:
         - si duck --> regarder si y'a une carte à newPosition, si oui --> retourner false, si non --> voir si on peut déplacer la carte là bas
           en regardant si y'a des cartes adjacentes à newPosition
         - si cover --> regarder si y'a une carte à newPosition, si non --> retourner false, si oui --> retourner true
         */
        public void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard)
        {
            GameCard? card = grid.GetCard(position);
            if (card == null)
                throw new Error(1);

            if (card.Number != currentDeckCard.Number)
                throw new Error(1);

            switch (funcName.ToLower())
            {
                case "duck":
                    GameCard? cardToDuck = grid.GetCard(newPosition);
                    if (cardToDuck != null)
                        throw new Error(1);
                    if (grid.IsAdjacentToCard(cardToDuck!.Position) == (false, null))
                        throw new Error(1);

                    return;

                case "cover":
                    GameCard? cardToCover = grid.GetCard(newPosition);
                    if (cardToCover == null)
                        throw new Error(1);

                    if (grid.IsAdjacentToCard(cardToCover.Position) == (false, null))
                        throw new Error(1);

                    return;

                default:
                    throw new Error(1);
            }
        }


        public bool IsGameOver(int cardPassed, int stackCounter) => (cardPassed == 8 || stackCounter == 1);

        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }
    }
}