using Models.Enums;
using System.Diagnostics.CodeAnalysis;
namespace Models.Exceptions

{
    [ExcludeFromCodeCoverage]
    public class ErrorHandler
    {
        
        private readonly Error _error;

        public ErrorHandler(Error error)
        {
            _error = error;
        }

        public string Handle()
        {
            return _error.ErrorCode switch
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
                _ => "Code d'erreur non reconnu."
            };
        }
    }
}