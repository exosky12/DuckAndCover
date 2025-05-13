using Model;
namespace UnitTests;

public class PositionTests
{
    [Fact]
    public void TestPositionConstructor()
    {
        Position pos = new Position(1, 2);

        Assert.Equal(1, pos.Row);
        Assert.Equal(2, pos.Column);
    }
    
    [Fact]
    public void TestPositionEquals()
    {
        Position pos1 = new Position(1, 2);
        Position pos2 = new Position(1, 2);

        Assert.True(pos1.Equals(pos2));
    }
    
    [Fact]
    public void TestPositionNotEqualsNull()
    {
        Position pos1 = new Position(1, 2);
        Position pos2 = new Position(2, 3);

        Assert.False(pos1.Equals(pos2));
    }
}