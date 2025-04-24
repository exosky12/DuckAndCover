using System;
using System.Collections.Generic;

namespace Model;

public class Grid
{
    public List<GameCard> GameCardsGrid;

    public Grid()
    {
        GridGenerator gridGenerator = new GridGenerator();
        GameCardsGrid = gridGenerator.Grid;
    }

    public (int minX, int maxX, int minY, int maxY) GetBounds(List<Position> positions)
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

    public GameCard GetCard(Position p)
    {
        foreach (var card in GameCardsGrid)
        {
            if (card.position.Row == p.Row && card.position.Column == p.Column)
            {
                return card;
            }
        }
        /* voir pour return jsp */
    }

    public void SetCard(Position p, GameCard newCard)
    {
        newCard.position = p;
    }

    public bool IsInGrid(Position p)
    {
        foreach (var card in GameCardsGrid)
        {
            if (card.position.Row == p.Row && card.position.Column == p.Column)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAdjacentToCard(Position p)
    {
        foreach (var card in GameCardsGrid)
        {
            var rowDiff = Math.Abs((int)card.position.Row - (int)p.Row);
            var colDiff = Math.Abs((int)card.position.Column - (int)p.Column);

            if ((rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1))
            {
                return true;
            }
        }

        return false;
    }
}