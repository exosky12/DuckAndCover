using Models.Enums;
using Models.Events;
using Models.Exceptions;

namespace Models.Game
{
    public class GameState
    {
        private GameStateEnum _currentState;
        private readonly Game _game;

        public GameStateEnum CurrentState => _currentState;

        public GameState(Game game)
        {
            _game = game;
            _currentState = GameStateEnum.NotStarted;
        }

        public void TransitionTo(GameStateEnum newState)
        {
            if (!IsValidTransition(newState))
            {
                throw new ErrorException(ErrorCodes.InvalidStateTransition, 
                    $"Transition invalide de {_currentState} vers {newState}");
            }

            _currentState = newState;
        }

        private bool IsValidTransition(GameStateEnum newState)
        {
            return _currentState switch
            {
                GameStateEnum.NotStarted => newState == GameStateEnum.WaitingForPlayerAction,
                GameStateEnum.WaitingForPlayerAction => newState is GameStateEnum.WaitingForCoverTarget 
                    or GameStateEnum.WaitingForDuckTarget 
                    or GameStateEnum.ProcessingCardEffect 
                    or GameStateEnum.GameOver,
                GameStateEnum.WaitingForCoverTarget => newState is GameStateEnum.ProcessingCardEffect 
                    or GameStateEnum.GameOver,
                GameStateEnum.WaitingForDuckTarget => newState is GameStateEnum.ProcessingCardEffect 
                    or GameStateEnum.GameOver,
                GameStateEnum.ProcessingCardEffect => newState is GameStateEnum.WaitingForPlayerAction 
                    or GameStateEnum.GameOver,
                GameStateEnum.GameOver => false,
                _ => false
            };
        }

        public bool CanPerformAction(string action)
        {
            return _currentState switch
            {
                GameStateEnum.WaitingForPlayerAction => action is "1" or "2" or "3" or "4" or "5" or "6",
                GameStateEnum.WaitingForCoverTarget => action == "1",
                GameStateEnum.WaitingForDuckTarget => action == "2",
                GameStateEnum.ProcessingCardEffect => false,
                GameStateEnum.GameOver => false,
                _ => false
            };
        }
    }
} 