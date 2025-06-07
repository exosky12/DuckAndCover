namespace Models.Enums
{
    /// <summary>
    /// Énumération des codes d'erreur possibles dans le jeu.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// La carte n'a pas été trouvée.
        /// </summary>
        CardNotFound = 1001,

        /// <summary>
        /// Aucune carte adjacente n'a été trouvée.
        /// </summary>
        AdjacentCardNotFound = 1002,

        /// <summary>
        /// Le numéro de la carte ne correspond pas au numéro de la carte du deck.
        /// </summary>
        CardNumberNotEqualToDeckCardNumber = 1003,

        /// <summary>
        /// Le nom de la fonction est invalide.
        /// </summary>
        InvalidFunctionName = 1004,

        /// <summary>
        /// Le format de la position est invalide.
        /// </summary>
        WrongPositionFormat = 1005,

        /// <summary>
        /// Les positions doivent être des nombres entiers.
        /// </summary>
        PositionsMustBeIntegers = 1006,

        /// <summary>
        /// Une carte existe déjà à cette position.
        /// </summary>
        CardAlreadyExists = 1007,

        /// <summary>
        /// Le choix effectué est invalide.
        /// </summary>
        InvalidChoice = 1008,

        /// <summary>
        /// Les cartes ne sont pas adjacentes.
        /// </summary>
        CardsAreNotAdjacent = 1009,

        /// <summary>
        /// Une erreur inconnue s'est produite.
        /// </summary>
        UnknownError = 1010,

        /// <summary>
        /// Le deck est vide.
        /// </summary>
        DeckEmpty = 1011,

        /// <summary>
        /// L'identifiant de la partie n'a pas été trouvé.
        /// </summary>
        GameIdNotFound = 1012,

        /// <summary>
        /// Ce n'est pas le tour du joueur.
        /// </summary>
        NotPlayerTurn = 1013,

        /// <summary>
        /// La transition d'état est invalide.
        /// </summary>
        InvalidStateTransition,

        /// <summary>
        /// L'action effectuée est invalide.
        /// </summary>
        InvalidAction
    }
}