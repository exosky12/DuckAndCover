using System;
using System.Collections.Generic;

public class Grid
{
    public List<GameCard> grid;

    public Grid()
    {
        grid = GenerateGrid();
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


    public GameCard GetCard(Position p)
    {
        foreach (var card in grid)
        {
            if (card.position.row == p.row && card.position.column == p.column)
            {
                return card;
            }
        }
    }

    public void SetCard(Position p, GameCard newCard)
    {
        newCard.position = p;
    }

    public bool IsInGrid(Position p)
    {
        foreach (var card in grid)
        {
            if (card.position.row == p.row && card.position.column == p.column)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAdjacentToCard(Position p)
    {
        foreach (var card in grid)
        {
            var rowDiff = Math.Abs((int)card.position.row - (int)p.row);
            var colDiff = Math.Abs((int)card.position.column - (int)p.column);

            if ((rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1))
            {
                return true;
            }
        }

        return false;
    }
}