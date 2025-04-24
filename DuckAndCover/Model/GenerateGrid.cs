namespace Model;

public class GenerateGrid : IGenerator
{
    public List<Card> Grid { get; private set; } = new List<Card>();

    /* ici faudra mettre une liste de cartes possibles pour le jeu (la grille du joueur) */
    public List<Card> AllPossibleCards { get; private set; } = new List<Card>();

    public int NbCards { get; private set; } = 12;

    public List<Card> GenerateGrid()
    {
        var rand = new Random();
        while (Grid.Count < NbCards)
        {
            var card = AllPossibleCards[rand.Next(1, AllPossibleCards.Count)];
            if (card is GameCard)
            {
                Grid.Add(card);
                AllPossibleCards.Remove(card);
            }
        }

        return Grid;
    }
}