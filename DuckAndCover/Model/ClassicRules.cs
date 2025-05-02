namespace Model;

public class ClassicRules : IRules
{
    public string Name => "Règles Classiques";

    public string Description =>
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris";

    public int NbCardsInDeck => 52;


    /* Check si y'a une carte à l'endroit de position --> si non alors retourner false, si oui --> regarder la méthode qui est appelé
     grâce à funcName:
     - si duck --> regarder si y'a une carte à newPosition, si oui --> retourner false, si non --> voir si on peut déplacer la carte là bas
       en regardant si y'a des cartes adjacentes à newPosition
     - si cover --> regarder si y'a une carte à newPosition, si non --> retourner false, si oui --> retourner true
     */
    public bool IsValidMove(Position position, Position newPosition, Grid grid, string funcName)
    {
        if (!grid.IsInGrid(position)) return false; // Vérification si la position d'origine est valide

        // Utilisation de switch pour gérer les cas de funcName
        switch (funcName.ToLower())
        {
            case "duck":
                // Si la nouvelle position est occupée, retourner false
                if (grid.GetCard(newPosition) != null) return false;

                // Vérification si une carte est adjacente à la nouvelle position
                var (isAdjacent, adjacentCard) = grid.IsAdjacentToCard(newPosition);
                return isAdjacent;

            case "cover":
                // Vérifier si la position de destination est occupée (nécessaire pour recouvrir)
                return grid.IsInGrid(newPosition) && grid.GetCard(newPosition) != null;

            default:
                return false; // Si funcName n'est ni "duck" ni "cover", retourner false
        }
    }


    public bool IsGameOver(int cardPassed, int stackCounter) => (cardPassed == 8 || stackCounter == 1);
}