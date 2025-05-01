namespace Model;

public class GridGenerator : IGenerator<GameCard>
{
    public List<GameCard> Grid { get; private set; }

    public List<Position> AllPositions { get; private set; } = InitializePositions();

    private static List<Position> InitializePositions()
    {
        var positions = new List<Position>();

        for (int row = 0; row < 3; row++)         // 3 lignes
        {
            for (int col = 0; col < 4; col++)     // 4 colonnes
            {
                positions.Add(new Position(row, col));
            }
        }

        return positions;
    }

    public int NbCards { get; private set; } = 12;

    public GridGenerator()
    {
        Grid = GenerateAllCards();
        Generate();
    }
    public List<GameCard> Generate()
    {
        var rand = new Random();
        var positionsCopy = new List<Position>(AllPositions);

        // Mélanger les cartes pour assurer une distribution aléatoire
        var shuffledGrid = Grid.OrderBy(card => rand.Next()).ToList();  // Mélange les cartes

        // Attribuer chaque carte à une position aléatoire
        for (int i = 0; i < shuffledGrid.Count; i++)
        {
            if (positionsCopy.Count == 0)
                break;

            var index = rand.Next(positionsCopy.Count);  // Choisir une position aléatoire
            var position = positionsCopy[index];

            shuffledGrid[i].Position = position;  // Assigner la position à la carte
            positionsCopy.RemoveAt(index);  // Retirer la position utilisée
        }

        // Mettre à jour Grid avec les cartes mélangées
        Grid = shuffledGrid;

        return Grid;
    }
    private static List<GameCard> GenerateAllCards()
    {
        var cards = new List<GameCard>();

        for (int number = 1; number <= 12; number++)
        {
            int splash = number switch
            {
                1 => 0,
                2 or 3 => 1,
                4 or 5 or 6 => 2,
                7 or 8 or 9 => 3,
                10 or 11 => 4,
                12 => 5,
                _ => throw new InvalidOperationException("Invalid card number")
            };

            cards.Add(new GameCard(splash, number));
        }

        return cards;
    }
}