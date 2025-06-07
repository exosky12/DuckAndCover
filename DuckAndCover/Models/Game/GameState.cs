using Models.Enums;
using Models.Events;
using Models.Exceptions;

namespace Models.Game
{
    public interface IGameState
    {
        GameStateEnum State { get; }
        bool CanPerformAction(string action);
        void HandleAction(string action);
        void Enter();
        void Exit();
    }

    public abstract class BaseGameState : IGameState
    {
        protected readonly Game Game;
        public abstract GameStateEnum State { get; }

        protected BaseGameState(Game game)
        {
            Game = game;
        }

        public abstract bool CanPerformAction(string action);
        public abstract void HandleAction(string action);

        public virtual void Enter() { }
        public virtual void Exit() { }
    }

    public class NotStartedState : BaseGameState
    {
        public override GameStateEnum State => GameStateEnum.NotStarted;

        public NotStartedState(Game game) : base(game) { }

        public override bool CanPerformAction(string action) => false;

        public override void HandleAction(string action)
        {
            throw new ErrorException(ErrorCodes.InvalidStateTransition, 
                "Aucune action possible dans l'état NotStarted");
        }
    }

    public class WaitingForPlayerActionState : BaseGameState
    {
        public override GameStateEnum State => GameStateEnum.WaitingForPlayerAction;

        public WaitingForPlayerActionState(Game game) : base(game) { }

        public override bool CanPerformAction(string action)
        {
            return action is "1" or "2" or "3" or "4" or "5" or "6";
        }

        public override void HandleAction(string action)
        {
            switch (action)
            {
                case "1":
                    Game.TransitionTo(GameStateEnum.WaitingForCoverTarget);
                    break;
                case "2":
                    Game.TransitionTo(GameStateEnum.WaitingForDuckTarget);
                    break;
                case "3":
                    Game.DoCoin(Game.CurrentPlayer);
                    break;
                case "4":
                    Game.DoStack(Game.CurrentPlayer);
                    break;
                case "5":
                    Game.DoQuit();
                    break;
                case "6":
                    Game.DoHelp();
                    break;
                default:
                    throw new ErrorException(ErrorCodes.InvalidAction, 
                        $"Action {action} non autorisée dans l'état {State}");
            }
        }
    }

    public class WaitingForCoverTargetState : BaseGameState
    {
        public override GameStateEnum State => GameStateEnum.WaitingForCoverTarget;

        public WaitingForCoverTargetState(Game game) : base(game) { }

        public override bool CanPerformAction(string action) => action == "1";

        public override void HandleAction(string action)
        {
            if (action != "1")
            {
                throw new ErrorException(ErrorCodes.InvalidAction, 
                    $"Action {action} non autorisée dans l'état {State}");
            }
            Game.TransitionTo(GameStateEnum.ProcessingCardEffect);
        }
    }

    public class WaitingForDuckTargetState : BaseGameState
    {
        public override GameStateEnum State => GameStateEnum.WaitingForDuckTarget;

        public WaitingForDuckTargetState(Game game) : base(game) { }

        public override bool CanPerformAction(string action) => action == "2";

        public override void HandleAction(string action)
        {
            if (action != "2")
            {
                throw new ErrorException(ErrorCodes.InvalidAction, 
                    $"Action {action} non autorisée dans l'état {State}");
            }
            Game.TransitionTo(GameStateEnum.ProcessingCardEffect);
        }
    }

    public class ProcessingCardEffectState : BaseGameState
    {
        public override GameStateEnum State => GameStateEnum.ProcessingCardEffect;

        public ProcessingCardEffectState(Game game) : base(game) { }

        public override bool CanPerformAction(string action) => false;

        public override void HandleAction(string action)
        {
            throw new ErrorException(ErrorCodes.InvalidAction, 
                "Aucune action possible pendant le traitement des effets");
        }

        public override void Enter()
        {
            Game.ProcessCardEffect();
            Game.TransitionTo(GameStateEnum.WaitingForPlayerAction);
        }
    }

    public class GameOverState : BaseGameState
    {
        public override GameStateEnum State => GameStateEnum.GameOver;

        public GameOverState(Game game) : base(game) { }

        public override bool CanPerformAction(string action) => false;

        public override void HandleAction(string action)
        {
            throw new ErrorException(ErrorCodes.InvalidAction, 
                "Aucune action possible dans l'état GameOver");
        }
    }

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

        public void ValidateAction(string action)
        {
            if (!CanPerformAction(action))
            {
                throw new ErrorException(ErrorCodes.InvalidAction, 
                    $"Action {action} non autorisée dans l'état {_currentState}");
            }
        }

        public void ValidatePlayerTurn(Player player)
        {
            if (player != _game.CurrentPlayer)
            {
                throw new ErrorException(ErrorCodes.InvalidPlayer, "Ce n'est pas votre tour");
            }
        }

        public void ValidateTargetCard(Card targetCard, Player player, string actionType)
        {
            if (targetCard == _game.CurrentDeckCard)
            {
                throw new ErrorException(ErrorCodes.InvalidTarget, 
                    $"Vous ne pouvez pas {actionType} la carte du deck");
            }

            if (!player.Grid.GameCardsGrid.Contains(targetCard))
            {
                throw new ErrorException(ErrorCodes.InvalidTarget, 
                    "La carte cible n'appartient pas à votre grille");
            }
        }
    }
} 