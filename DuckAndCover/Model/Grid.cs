using System;
using System.Collections.Generic;

public class Grid
{
    public List<List<GameCard>> GridMatrix;

    public Grid(int rows, int columns)
    {
        GridMatrix = new List<List<GameCard>>();
        for (int i = 0; i < rows; i++)
        {
            var row = new List<GameCard>();
            for (int j = 0; j < columns; j++)
            {
                var gameCard = new GameCard(i, j);
                row.Add(gameCard);
            }
            GridMatrix.Add(row);
        }
    }



    public (int minX, int maxX, int minY, int maxY) GetBounds(List<Position> positions)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var pos in positions)
        {
            if (pos.column < minX) minX = pos.column;
            if (pos.column > maxX) maxX = pos.column;
            if (pos.row < minY) minY = pos.row;
            if (pos.row > maxY) maxY = pos.row;
        }

        return (minX, maxX, minY, maxY);
    }
}

 public GameCard GetCard(Position p)
    {
        return GridMatrix[p.row][p.column];
    }

    public void SetCard(Position p, GameCard newCard)
    {
        GridMatrix[p.row][p.column] = newCard;
    }

