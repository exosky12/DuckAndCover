using System;
using GameModel = Models.Game.Game;
using System.Diagnostics.CodeAnalysis;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class GameStartedEventArgs : EventArgs
    {
        public GameModel Game { get; }

        public GameStartedEventArgs(GameModel game) => Game = game;
    }

    [ExcludeFromCodeCoverage]
    public class GameResumedEventArgs : EventArgs
    {
        public GameModel Game { get; }

        public GameResumedEventArgs(GameModel game) => Game = game;
    }
}
