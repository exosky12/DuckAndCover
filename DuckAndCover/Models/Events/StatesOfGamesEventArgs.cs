using System;
using GameModel = Models.Game.Game;

namespace Models.Events
{
    [ExcludeFromCodeCoverage]
    public class GameStartedEventArgs : EventArgs
    {
        public GameModel Game { get; }

        public GameStartedEventArgs(GameModel game) => Game = game;
    }

    /// <summary>
    /// Event args for when an existing game is resumed.
    /// </summary>
    public class GameResumedEventArgs : EventArgs
    {
        public GameModel Game { get; }

        public GameResumedEventArgs(GameModel game) => Game = game;
    }
}
