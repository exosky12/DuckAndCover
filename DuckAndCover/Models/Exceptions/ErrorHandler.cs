using Models.Enums;
using System.Diagnostics.CodeAnalysis;
namespace Models.Exceptions
{
    /// <summary>
    /// Gestionnaire d'erreurs qui convertit les codes d'erreur en messages explicites.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ErrorHandler
    {
        /// <summary>
        /// L'erreur à traiter.
        /// </summary>
        private readonly ErrorException _errorException;

        /// <summary>
        /// Initialise une nouvelle instance de la classe ErrorHandler.
        /// </summary>
        /// <param name="errorException">L'erreur à traiter.</param>
        public ErrorHandler(ErrorException errorException)
        {
            _errorException = errorException;
        }

        /// <summary>
        /// Traite l'erreur et retourne un message explicite correspondant au code d'erreur.
        /// </summary>
        /// <returns>Un message explicite décrivant l'erreur.</returns>
        public string Handle()
        {
            return _errorException.ErrorCode switch
            {
                ErrorCodes.CardNotFound => "La carte est introuvable à la position donnée.",
                ErrorCodes.AdjacentCardNotFound => "Aucune carte adjacente. Vous devez donner une position avec au moins une carte adjacente.",
                ErrorCodes.CardNumberNotEqualToDeckCardNumber => "Le numéro de la carte ne correspond pas à celui attendu dans le deck.",
                ErrorCodes.InvalidFunctionName => "Le nom de la fonction est invalide.",
                ErrorCodes.WrongPositionFormat => "Le format de la position est incorrect. Il doit ressembler à 'ligne,colonne'.",
                ErrorCodes.PositionsMustBeIntegers => "Les positions doivent être des entiers.",
                ErrorCodes.CardAlreadyExists => "Une carte existe déjà à cette position. Duck impossible.",
                ErrorCodes.CardsAreNotAdjacent => "Les cartes ne sont pas adjacentes. Cover impossible",
                ErrorCodes.DeckEmpty => "Le deck est vide. Impossible de tirer une carte.",
                ErrorCodes.GameIdNotFound => "La partie est introuvable.",
                ErrorCodes.InvalidChoice => "Choix invalide. Veuillez réessayer.",
                ErrorCodes.UnknownError => "Une erreur inconnue s'est produite.",
                ErrorCodes.NotPlayerTurn => "Ce n'est pas le tour du joueur.",
                ErrorCodes.InvalidOperation => "Opération invalide dans le contexte actuel.",
                ErrorCodes.ArgumentNull => "Argument requis non fourni ou null.",
                ErrorCodes.InvalidCardNumber => "Le numéro de carte fourni est invalide.",
                ErrorCodes.GameManagerNotInitialized => "Le GameManager doit être initialisé avant utilisation.",
                _ => "Code d'erreur non reconnu."
            };
        }
    }
}