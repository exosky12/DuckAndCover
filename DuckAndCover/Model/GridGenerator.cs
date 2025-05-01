namespace Model;

public class GridGenerator : IGenerator<GameCard>
{
    public List<GameCard> Grid { get; private set; } = new List<GameCard>();
    public List<GameCard> AllPossibleCards { get; private set; } = new List<GameCard>();
    public int NbCards { get; private set; } = 12;

    public GridGenerator()
    {
        Generate();
    }

    private List<GameCard> Generate()
    {
        var rand = new Random();
        while (Grid.Count < NbCards)
        {
            var card = AllPossibleCards[rand.Next(0, AllPossibleCards.Count)];
            Grid.Add(card);
            AllPossibleCards.Remove(card);
        }

        return Grid;
    }
}