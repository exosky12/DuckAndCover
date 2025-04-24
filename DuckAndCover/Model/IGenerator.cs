namespace Model;

public interface IGenerator
{
    List<Card> Grid { get; }

    List<Card> AllPossibleCards { get; }

    int NbCards { get; }
}