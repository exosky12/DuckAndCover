#pragma warning disable SYSLIB005

using Models.Enums;

namespace Models.Exceptions
{
    /// <summary>
    /// Classe d'exception personnalisée pour gérer les erreurs du jeu.
    /// </summary>
    public class ErrorException : Exception
    {
        /// <summary>
        /// Obtient le code d'erreur associé à l'exception.
        /// </summary>
        public ErrorCodes ErrorCode { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Error.
        /// </summary>
        /// <param name="errorCode">Le code d'erreur associé à l'exception.</param>
        /// <param name="message">Le message d'erreur (toujours vide mais nécessaire car ça dérive de la classe Exception).</param>
        public ErrorException(ErrorCodes errorCode, string message = "") : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}