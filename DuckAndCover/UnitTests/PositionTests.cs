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

    [Fact]
    public void TestPositionEqualsNull()
    {
        Position pos = new Position(1, 2);

        Assert.False(pos.Equals(null));
    }

    [Fact]
    public void TestPositionEqualsDifferentType()
    {
        Position pos = new Position(1, 2);

        Assert.False(pos.Equals("Not a Position")); 
    }

    [Fact]
    public void TestPositionGetHashCode()
    {
        Position pos1 = new Position(1, 2);
        Position pos2 = new Position(1, 2);

        // Vérifie que deux positions égales ont le même hash code
        Assert.Equal(pos1.GetHashCode(), pos2.GetHashCode());
    }

    [Fact]
    public void TestPositionGetHashCodeDifferentPositions()
    {
        Position pos1 = new Position(1, 2);
        Position pos2 = new Position(2, 3);

        Assert.NotEqual(pos1.GetHashCode(), pos2.GetHashCode());
    }
}