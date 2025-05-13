using Model;
namespace UnitTests;

public class GridTest
{
    /* Il reste à tester:
        - Grid constructeur (nécessite de test le générateur avec injection de dépendance et FakeGenerator)
     */
    [Fact]
    public void TestGridSize()
    {
        Grid grid = new Grid();
        
        int gridSize = grid.GameCardsGrid.Count;

        Assert.Equal(12, gridSize);
    }
    
    [Fact]
    public void TestGetBounds()
    {
        Grid grid = new Grid();
        List<Position> positions = new List<Position>();
        grid.GameCardsGrid.ForEach((card =>
        {
            positions.Add(card.Position);
        }));

        var bounds = Grid.GetBounds(positions);
        
        Assert.Equal(1, bounds.minX);
        Assert.Equal(4, bounds.maxX);
        Assert.Equal(1, bounds.minY);
        Assert.Equal(3, bounds.maxY);
    }

    [Fact]
    public void TestGetCard_WhenCardAtThisPosition()
    {
        Grid grid = new Grid();
        GameCard card = new GameCard(4, 12);
        card.Position = new Position(2, 2);
        grid.SetCard(card.Position, card);
        
        GameCard? cardAtPosition = grid.GetCard(card.Position);
        
        Assert.Equal(card, cardAtPosition);
    }
    
    [Fact]
    public void TestGetCard_WhenNoCardAtThisPosition()
    {
        Grid grid = new Grid();
        
        GameCard? card = grid.GetCard(new Position(10, 10));
        
        Assert.Null(card);
    }

    [Fact]
    public void TestSetCard()
    {
        Grid grid = new Grid();
        GameCard card = new GameCard(4, 12);
        card.Position = new Position(2, 2);
        
        grid.SetCard(card.Position, card);
        GameCard? cardAtPosition = grid.GetCard(card.Position);
        
        Assert.Equal(card, cardAtPosition);
    }
    
    [Fact]
    public void TestIsInGrid_WhenCardIsInGrid()
    {
        Grid grid = new Grid();
        GameCard card = new GameCard(4, 12);
        card.Position = new Position(2, 2);
        grid.SetCard(card.Position, card);

        bool isInGrid = grid.IsInGrid(card.Position);
        
        Assert.True(isInGrid);
    }
    
    [Fact]
    public void TestIsInGrid_WhenCardIsNotInGrid()
    {
        Grid grid = new Grid();
        GameCard card = new GameCard(4, 12);
        card.Position = new Position(10, 10);

        bool isInGrid = grid.IsInGrid(card.Position);
        
        Assert.False(isInGrid);
    }

    [Fact]
    public void TestIsAdjacentToCard()
    {
        Grid grid = new Grid();
        GameCard card1 = new GameCard(4, 12);
        card1.Position = new Position(6, 6);
        GameCard card2 = new GameCard(4, 12);
        card2.Position = new Position(6, 7);
        grid.SetCard(card1.Position, card1);
        grid.SetCard(card2.Position, card2);
        
        (bool isAdjacent, GameCard? adjacentCard) = grid.IsAdjacentToCard(card1.Position);
        
        Assert.True(isAdjacent);
        Assert.Equal(card2, adjacentCard);
    }
    
    
    [Fact]
    public void TestIsNotAdjacentToCard()
    {
        Grid grid = new Grid();
        GameCard card1 = new GameCard(4, 12);
        card1.Position = new Position(6, 6);
        grid.SetCard(card1.Position, card1);
        
        (bool isAdjacent, GameCard? adjacentCard) = grid.IsAdjacentToCard(card1.Position);
        
        Assert.False(isAdjacent);
        Assert.Null(adjacentCard);
    }

    [Fact]
    public void TestRemoveCard()
    {
        Grid grid = new Grid();
        GameCard card = new GameCard(4, 12);
        card.Position = new Position(2, 2);
        grid.SetCard(card.Position, card);
        
        grid.RemoveCard(card.Position);
        GameCard? cardAtPosition = grid.GetCard(card.Position);
        
        Assert.Null(cardAtPosition);
    }
}