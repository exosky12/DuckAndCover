// File: Models/Events/StatesOfGamesEventArgs.cs
using System;
using GameModel = Models.Game.Game;

namespace Models.Events
{
    /// <summary>
    /// Event args for when a new game is started.
    /// </summary>
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
