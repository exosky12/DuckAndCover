namespace Model
{
    
    public interface IGenerator<T>
    {
        List<T> Generate();

    }
}