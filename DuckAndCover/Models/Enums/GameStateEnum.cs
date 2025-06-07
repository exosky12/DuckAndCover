namespace Models.Enums
{
    public enum GameStateEnum
    {
        NotStarted,
        WaitingForPlayerAction,
        WaitingForCoverTarget,
        WaitingForDuckTarget,
        ProcessingCardEffect,
        GameOver
    }
} 