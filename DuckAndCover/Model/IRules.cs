namespace Model;

public interface IRules
{
    string Name { get; }

    bool IsValidMove(Position position, Position newPosition, Grid grid, string funcName)
    {
        /* Check si y'a une carte à l'endroit de position --> si non alors retourner false, si oui --> regarder la méthode qui est appelé
         grâce à funcName:
         - si duck --> regarder si y'a une carte à newPosition, si oui --> retourner false, si non --> voir si on peut déplacer la carte là bas
           en regardant si y'a des cartes adjacentes à newPosition
         - si cover --> regarder si y'a une carte à newPosition, si non --> retourner false, si oui --> retourner true
         */
    }
    
    
}