namespace Model;

public class Position
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    protected Position(int row, int column)
    {
        this.Row = row;
        this.Column = column;
    }
}