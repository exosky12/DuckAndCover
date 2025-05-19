using System.Diagnostics.CodeAnalysis;
using Models.Exceptions;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class ErrorOccurredEventArgs
    {
        public Error Error { get; }

        public ErrorOccurredEventArgs(Error error)
        {
            Error = error;
        }
    }
}