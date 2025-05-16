namespace Models.Interfaces
{
    public interface IGenerator<T>
    {
        List<T> Generate();
    }
}