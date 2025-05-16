using Models.Generators;

namespace Models.Game
{
    public class Grid
    {
        public List<GameCard> GameCardsGrid { get; }


        public Grid()
        {
            GridGenerator gridGenerator = new GridGenerator();
            GameCardsGrid = gridGenerator.Grid;
        }

        public static (int minX, int maxX, int minY, int maxY) GetBounds(List<Position> positions)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;

            foreach (var pos in positions)
            {
                if (pos.Column < minX) minX = pos.Column;
                if (pos.Column > maxX) maxX = pos.Column;
                if (pos.Row < minY) minY = pos.Row;
                if (pos.Row > maxY) maxY = pos.Row;
            }

            return (minX, maxX, minY, maxY);
        }


        public GameCard? GetCard(Position p)
        {
            foreach (var card in GameCardsGrid)
            {
                if (card.Position.Row == p.Row && card.Position.Column == p.Column)
                {
                    return card;
                }
            }

            return null;
        }

        public void SetCard(Position p, GameCard newCard)
        {
            if (IsInGrid(p))
            {
                RemoveCard(p);
            }

            newCard.Position = p;
            GameCardsGrid.Add(newCard);
        }

        public bool IsInGrid(Position p)
        {
            return GameCardsGrid
                .Select(card => card.Position)
                .Any(pos => pos.Row == p.Row && pos.Column == p.Column);
        }

        public (bool isAdjacent, GameCard? adjacentCard) IsAdjacentToCard(Position p)
        {
            foreach (var card in GameCardsGrid)
            {
                var rowDiff = Math.Abs(card.Position.Row - p.Row);
                var colDiff = Math.Abs(card.Position.Column - p.Column);

                if ((rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1))
                {
                    return (true, card);
                }
            }

            return (false, null);
        }

        public void RemoveCard(Position p)
        {
            bool cardFound = IsInGrid(p);
            if (cardFound)
            {
                GameCardsGrid.RemoveAll(card => card.Position.Row == p.Row && card.Position.Column == p.Column);
            }
        }
    }
}