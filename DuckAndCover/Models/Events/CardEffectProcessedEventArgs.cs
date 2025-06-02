using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class CardEffectProcessedEventArgs : EventArgs
    {
        public string Message { get; }
        public DeckCard ProcessedCard { get; }

        public CardEffectProcessedEventArgs(string message, DeckCard processedCard)
        {
            Message = message;
            ProcessedCard = processedCard;
        }
    }
}