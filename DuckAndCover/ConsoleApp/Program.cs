// File: ConsoleApp/Program.cs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
using Models.Exceptions;
using DataPersistence;
using Models.Interfaces;
using Models.Enums;
using static System.Console;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main()
        {
            Utils.ShowTitle();

            var stub = new Stub();
            var (_, stubGames) = stub.LoadData();

            Game game = GetGameFromUserChoice(stubGames);

            if (game == null)
                throw new Error(ErrorCodes.UnknownError);

            SubscribeToGameEvents(game);
            game.GameLoop();
        }

        private static Game GetGameFromUserChoice(List<Game> stubGames)
        {
            while (true)
            {
                Write("Souhaitez-vous reprendre une partie en cours ? (O/N) ");
                var answer = ReadLine()?.Trim().ToUpperInvariant();

                switch (answer)
                {
                    case "O": return ResumeGame(stubGames);
                    case "N": return Utils.CreateNewGame();
                    default:
                        var handler = new ErrorHandler(new Error(ErrorCodes.InvalidChoice));
                        Utils.WriteError(handler.Handle());
                        WriteLine("Appuyez sur une touche pour continuer...");
                        ReadKey(true);
                        break;
                }
            }
        }

        private static Game ResumeGame(List<Game> stubGames)
        {
            var inProgress = stubGames.Where(g => !g.IsFinished).ToList();

            if (inProgress.Count == 0)
            {
                WriteLine("Aucune partie en cours disponible.");
                return Utils.CreateNewGame();
            }

            while (true)
            {
                WriteLine("Parties disponibles (Code – Joueurs) :");
                foreach (var g in inProgress)
                    WriteLine($"{g.Id} – {string.Join(", ", g.Players.Select(p => p.Name))}");

                Write("Entrez le code de la partie à reprendre : ");
                var codeInput = (ReadLine() ?? "").Trim().ToUpperInvariant();

                var resumed = inProgress.FirstOrDefault(g =>
                    g.Id.Equals(codeInput, StringComparison.OrdinalIgnoreCase));

                if (resumed != null)
                {
                    Game.RaiseGameResumed(resumed);
                    return resumed;
                }

                var handler = new ErrorHandler(new Error(ErrorCodes.GameIdNotFound));
                Utils.WriteError(handler.Handle());
                WriteLine("Appuyez sur une touche pour continuer...");
                ReadKey(true);
            }
        }

        private static void SubscribeToGameEvents(Game game)
        {
            game.PlayerChanged += (s, e) =>
            {
                var choice = Utils.PromptPlayerTurn(e.CurrentPlayer, e.DeckCard, game);
                game.HandlePlayerChoice(e.CurrentPlayer, choice);
            };

            game.DisplayMenuNeeded += (s, e) =>
            {
                var choice = Utils.PromptPlayerTurn(e.CurrentPlayer, e.DeckCard, game);
                game.HandlePlayerChoice(e.CurrentPlayer, choice);
            };

            game.PlayerChooseCoin += (s, e) =>
                Utils.WriteGameMaster($"{e.Player.Name} a fait coin !");

            game.PlayerChooseCover += (s, e) =>
            {
                Utils.WriteGameMaster("Quelle carte souhaitez‑vous utiliser pour recouvrir ?");
                var src = ReadLine()!;
                Utils.WriteGameMaster("Quelle carte souhaitez‑vous recouvrir ?");
                var dst = ReadLine()!;
                game.HandlePlayerChooseCover(e.Player,
                    Utils.ParsePosition(src),
                    Utils.ParsePosition(dst));
            };

            game.PlayerChooseDuck += (s, e) =>
            {
                Utils.WriteGameMaster("Quelle carte souhaitez‑vous déplacer ?");
                var src = ReadLine()!;
                Utils.WriteGameMaster("Où souhaitez‑vous la déplacer ?");
                var dst = ReadLine()!;
                game.HandlePlayerChooseDuck(e.Player,
                    Utils.ParsePosition(src),
                    Utils.ParsePosition(dst));
            };

            game.PlayerChooseShowPlayersGrid += (s, e) =>
            {
                Utils.WriteGameMaster("Voici les grilles des joueurs :");
                foreach (var p in e.Players)
                {
                    Utils.WriteGameMaster($"Grille de {p.Name} :");
                    Utils.DisplayGrid(p);
                }
            };

            game.PlayerChooseShowScores += (s, e) =>
                Utils.DisplayPlayerScores(e.Players);

            game.PlayerChooseQuit += (s, e) =>
            {
                Utils.WriteGameMaster($"{e.Player.Name} a quitté la partie.");
                game.Quit = true;
            };

            game.GameIsOver += (s, e) =>
            {
                game.IsFinished = true;
                Utils.EndGame(game.Players, game);
            };

            game.ErrorOccurred += (s, e) =>
            {
                var handler = new ErrorHandler(e.Error);
                Utils.WriteError(handler.Handle());
                WriteLine("Appuyez sur une touche pour continuer...");
                ReadKey(true);
            };
        }
    }
}

