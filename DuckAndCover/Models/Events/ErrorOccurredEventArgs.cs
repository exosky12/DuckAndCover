using System.Diagnostics.CodeAnalysis;
using Models.Exceptions;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class ErrorOccurredEventArgs : EventArgs
    {
        public Error Error { get; }

        public ErrorOccurredEventArgs(Error error)
        {
            Error = error;
        }
    }
}