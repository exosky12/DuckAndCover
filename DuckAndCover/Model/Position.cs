namespace Model;

public class Position
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public Position(int row, int column)
    {
        this.Row = row;
        this.Column = column;
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as Position);
    }

    public bool Equals(Position? pos2)
    {
        if (pos2 is null) return false;
        return Row == pos2.Row && Column == pos2.Column;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}