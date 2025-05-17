using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
using Models.Exceptions;
using DataPersistence;
using static System.Console;
using Models.Interfaces;
using Models.Enums;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main()
        {
            Utils.ShowTitle();

            // Charge le stub
            IDataPersistence persistence = new Stub();
            var (stubPlayers, stubGames) = persistence.LoadData();

            Write("Souhaitez-vous reprendre une partie en cours ? (O/N) ");
            var answer = ReadLine()?.Trim().ToUpperInvariant();

            Game game = null;
            switch (answer)
            {
                case "O":
                    try
                    {
                        var inProgress = stubGames.Where(g => !g.IsFinished).ToList();
                        if (!inProgress.Any())
                        {
                            WriteLine("Aucune partie en cours disponible.");
                            game = Utils.CreateNewGame();
                        }
                        else
                        {
                            WriteLine("Parties disponibles (Code 5 — Joueurs) :");
                            foreach (var g in inProgress)
                            {
                                var code5 = g.Id.ToString().Substring(0, 5);
                                WriteLine($"{code5} — {string.Join(", ", g.Players.Select(p => p.Name))}");
                            }

                            Write("Entrez les 5 premiers caractères du code de la partie : ");
                            var codeInput = ReadLine()?.Trim() ?? "";

                            var resumed = inProgress
                                .FirstOrDefault(g =>
                                    g.Id.ToString().StartsWith(codeInput, StringComparison.OrdinalIgnoreCase));

                            if (resumed != null)
                            {
                                game = resumed;
                            }
                            else
                            {
                                throw new Error(ErrorCodes.GameIdNotFound);
                            }
                        }
                    }
                    catch (Error e)
                    {
                        var errorHandler = new ErrorHandler(e);
                        Utils.WriteError(errorHandler.Handle());
                        WriteLine("Appuyez sur une touche pour continuer...");
                        ReadKey(true);
                    }

                    break;

                case "N":
                    game = Utils.CreateNewGame();
                    break;

                default:
                    var handler = new ErrorHandler(new Error(ErrorCodes.InvalidChoice));
                    Utils.WriteError(handler.Handle());
                    WriteLine("Appuyez sur une touche pour continuer...");
                    ReadKey(true);
                    break;
            }

            if (game != null)
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
                game.PlayerChooseCoin += (s, e) => Utils.WriteGameMaster($"{e.Player.Name} a fait coin !");
                game.PlayerChooseCover += (s, e) =>
                {
                    Utils.WriteGameMaster("Quelle carte souhaitez‑vous utiliser pour recouvrir?");
                    Utils.WriteGameMaster("Entrez la position (ligne,colonne) — ex : 1,1");
                    var src = ReadLine()!;
                    Utils.WriteGameMaster("Quelle carte souhaitez‑vous recouvrir?");
                    Utils.WriteGameMaster("Entrez la position (ligne,colonne) — ex : 1,2");
                    var dst = ReadLine()!;
                    game.HandlePlayerChooseCover(e.Player, Utils.ParsePosition(src), Utils.ParsePosition(dst));
                };
                game.PlayerChooseDuck += (s, e) =>
                {
                    Utils.WriteGameMaster("Quelle carte souhaitez‑vous déplacer?");
                    Utils.WriteGameMaster("Entrez la position (ligne,colonne) — ex : 1,1");
                    var src = ReadLine()!;
                    Utils.WriteGameMaster("Où souhaitez‑vous la déplacer? (ligne,colonne) — ex : 2,3");
                    var dst = ReadLine()!;
                    game.HandlePlayerChooseDuck(e.Player, Utils.ParsePosition(src), Utils.ParsePosition(dst));
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
                game.PlayerChooseShowScores += (s, e) => Utils.DisplayPlayerScores(e.Players);
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

                // 3) Lancement de la boucle de jeu
                game.GameLoop();
            }
        }
    }
}