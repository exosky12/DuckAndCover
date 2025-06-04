using System.Runtime.Serialization;
using Models.Enums;

namespace Models.Exceptions
{
    /// <summary>
    /// Classe d'exception personnalisée pour gérer les erreurs du jeu.
    /// </summary>
    [Serializable]
    public class ErrorException : Exception, ISerializable
    {
        /// <summary>
        /// Obtient le code d'erreur associé à l'exception.
        /// </summary>
        public ErrorCodes ErrorCode { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe ErrorException.
        /// </summary>
        /// <param name="errorCode">Le code d'erreur associé à l'exception.</param>
        /// <param name="message">Le message d'erreur (toujours vide mais nécessaire car ça dérive de la classe Exception).</param>
        public ErrorException(ErrorCodes errorCode, string message = "") : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructeur de désérialisation.
        /// </summary>
        protected ErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var value = info.GetValue(nameof(ErrorCode), typeof(ErrorCodes));
            if (value is null)
                throw new SerializationException("ErrorCode is missing during deserialization.");
            ErrorCode = (ErrorCodes)value;
        }

        /// <summary>
        /// Surcharge de la méthode pour sérialiser les données personnalisées de l'exception.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(ErrorCode), ErrorCode, typeof(ErrorCodes));
            base.GetObjectData(info, context);
        }
    }
}