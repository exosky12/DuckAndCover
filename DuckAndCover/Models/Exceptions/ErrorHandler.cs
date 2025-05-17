using Models.Enums;
namespace Models.Exceptions
{
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
                _ => "Une erreur inconnue est survenue."
            };
        }
    }
}