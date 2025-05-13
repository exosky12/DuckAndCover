namespace Model;

public sealed class Position : IEquatable<Position>
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public Position(int row, int column)
    {
        this.Row = row;
        this.Column = column;
    }

    // Implémentation de la méthode Equals de IEquatable<Position>
    public bool Equals(Position? other)
    {
        if (other is null) return false;
        return Row == other.Row && Column == other.Column;
    }

    // Redéfinition de Equals (non générique) pour gérer les objets de type object
    public override bool Equals(object? obj)
    {
        return Equals(obj as Position);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}