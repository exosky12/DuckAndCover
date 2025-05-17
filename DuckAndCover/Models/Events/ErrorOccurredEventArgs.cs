using Models.Exceptions;
namespace Models.Events;

public class ErrorOccurredEventArgs
{
    public Error Error { get; }

    public ErrorOccurredEventArgs(Error error)
    {
        Error = error;
    }
}