namespace Models.Enums
{
    public enum ErrorCodes
    {
        CardNotFound = 1001,
        AdjacentCardNotFound = 1002,
        CardNumberNotEqualToDeckCardNumber = 1003,
        InvalidFunctionName = 1004,
        WrongPositionFormat = 1005,
        PositionsMustBeIntegers = 1006,
        CardAlreadyExists = 1007,
        InvalidChoice = 1008,
        CardsAreNotAdjacent = 1009,
        UnknownError = 1010,
        DeckEmpty = 1011,
        GameIdNotFound = 1012,
    }
}