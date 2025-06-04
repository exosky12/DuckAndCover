using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour le traitement d'un effet de carte.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CardEffectProcessedEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient le message décrivant l'effet de la carte.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Obtient la carte dont l'effet a été traité.
        /// </summary>
        public DeckCard ProcessedCard { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe CardEffectProcessedEventArgs.
        /// </summary>
        /// <param name="message">Le message décrivant l'effet de la carte.</param>
        /// <param name="processedCard">La carte dont l'effet a été traité.</param>
        public CardEffectProcessedEventArgs(string message, DeckCard processedCard)
        {
            Message = message;
            ProcessedCard = processedCard;
        }
    }
}