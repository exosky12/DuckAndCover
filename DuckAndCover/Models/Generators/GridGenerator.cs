using System.Security.Cryptography;
using Models.Game;
using Models.Interfaces;

namespace Models.Generators
{

    public class GridGenerator : IGenerator<GameCard>
    {
        public List<GameCard> Grid { get; private set; }
        public List<Position> AllPositions { get; private set; } = InitializePositions();
        public int NbCards { get; private set; } = 12;

        public GridGenerator()
        {
            Grid = GenerateAllCards();
            Generate();
        }

        public List<GameCard> Generate()
        {
            var positionsCopy = new List<Position>(AllPositions);
            var shuffledGrid =
                Grid.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToList();

            for (int i = 0; i < shuffledGrid.Count && positionsCopy.Count > 0; i++)
            {
                int index = RandomNumberGenerator.GetInt32(positionsCopy.Count);
                var position = positionsCopy[index];

                shuffledGrid[i].Position = position;
                positionsCopy.RemoveAt(index);
            }

            Grid = shuffledGrid;
            return Grid;
        }

        private static List<Position> InitializePositions()
        {
            var positions = new List<Position>();
            for (int row = 1; row <= 3; row++) // 3 lignes
            {
                for (int col = 1; col <= 4; col++) // 4 colonnes
                {
                    positions.Add(new Position(row, col));
                }
            }

            return positions;
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
}