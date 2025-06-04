using System.Diagnostics.CodeAnalysis;

namespace Models.Events
{
    /// <summary>
    /// Arguments d'événement pour la fin d'une partie.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GameIsOverEventArgs : EventArgs
    {
        /// <summary>
        /// Obtient une valeur indiquant si la partie est terminée.
        /// </summary>
        public bool IsOver { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe GameIsOverEventArgs.
        /// </summary>
        /// <param name="isOver">Indique si la partie est terminée.</param>
        public GameIsOverEventArgs(bool isOver)
        {
            IsOver = isOver;
        }
    }
}