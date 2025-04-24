namespace Model;

public class ClassicRules : IRules
{
    public string Name => "Règles Classiques";

    public string Description =>
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris"

    public int NbCardsInDeck => 52;

    public bool IsValidMove(Position position, Position newPosition, Grid grid, string funcName)
    {
        if (grid.IsInGrid(position))
        {
            if (funcName == "duck")
            {
                if (grid.IsInGrid(newPosition))
                {
                    return false;
                }

                // Check if there are adjacent cards to newPosition
                return grid.IsAdjacentToCard(newPosition);
            }
            else if (funcName == "cover")
            {
                if (grid.IsInGrid(newPosition))
                {
                    return true;
                }

                return false;
            }
        }

        return false;

        /* Check si y'a une carte à l'endroit de position --> si non alors retourner false, si oui --> regarder la méthode qui est appelé
         grâce à funcName:
         - si duck --> regarder si y'a une carte à newPosition, si oui --> retourner false, si non --> voir si on peut déplacer la carte là bas
           en regardant si y'a des cartes adjacentes à newPosition
         - si cover --> regarder si y'a une carte à newPosition, si non --> retourner false, si oui --> retourner true
         */
    }

    public bool IsGameOver(int cardPassed, int stackCounter) => (cardPassed == 8 || stackCounter == 1);
}