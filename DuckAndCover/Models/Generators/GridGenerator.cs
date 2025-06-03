using System.Security.Cryptography;
using Models.Game;
using Models.Interfaces;

namespace Models.Generators
{
    /// <summary>
    /// Générateur de grille de jeu avec des cartes placées aléatoirement.
    /// </summary>
    public class GridGenerator : IGenerator<GameCard>
    {
        /// <summary>
        /// Obtient la grille générée.
        /// </summary>
        public List<GameCard> Grid { get; private set; }

        /// <summary>
        /// Obtient la liste de toutes les positions possibles sur la grille.
        /// </summary>
        public List<Position> AllPositions { get; private set; } = InitializePositions();

        /// <summary>
        /// Obtient le nombre de cartes dans la grille.
        /// </summary>
        public int NbCards { get; private set; } = 12;

        /// <summary>
        /// Initialise une nouvelle instance de la classe GridGenerator.
        /// </summary>
        public GridGenerator()
        {
            Grid = GenerateAllCards();
            Generate();
        }

        /// <summary>
        /// Génère une nouvelle grille avec les cartes placées aléatoirement.
        /// </summary>
        /// <returns>La liste des cartes placées dans la grille.</returns>
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

        /// <summary>
        /// Initialise la liste des positions possibles sur la grille.
        /// </summary>
        /// <returns>Une liste contenant toutes les positions possibles (3x4).</returns>
        private static List<Position> InitializePositions()
        {
            var positions = new List<Position>();
            for (int row = 1; row <= 3; row++)
            {
                for (int col = 1; col <= 4; col++)
                {
                    positions.Add(new Position(row, col));
                }
            }

            return positions;
        }

        /// <summary>
        /// Génère toutes les cartes avec leurs valeurs de splash correspondantes.
        /// </summary>
        /// <returns>Une liste contenant toutes les cartes du jeu.</returns>
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