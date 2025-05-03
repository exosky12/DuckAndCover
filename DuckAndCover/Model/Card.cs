namespace Model;

public abstract class Card
{
    public int Number { get; private set; }

    protected Card(int number)
    {
        this.Number = number;
    }
}