namespace Model;

public interface IGenerator
{
    List<Card> Grid { get; }

    /* ici faudra mettre une liste de cartes possibles pour le jeu */
    List<Card> AllPossibleCards { get; }
}