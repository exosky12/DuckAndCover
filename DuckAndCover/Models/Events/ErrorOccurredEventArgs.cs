using System.Diagnostics.CodeAnalysis;
using Models.Exceptions;

namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour la survenue d'une erreur.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ErrorOccurredEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient l'erreur qui s'est produite.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe ErrorOccurredEventArgs.
        /// </summary>
        /// <param name="error">L'erreur qui s'est produite.</param>
        public ErrorOccurredEventArgs(Error error)
        {
            Error = error;
        }
    }
}