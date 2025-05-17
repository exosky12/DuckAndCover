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

            // 1) Charger les parties depuis le Stub
            IDataPersistence persistence = new Stub();
            var (_, stubGames) = persistence.LoadData();

            // 2) Demander reprise ou nouvelle partie
            Write("Souhaitez-vous reprendre une partie en cours ? (O/N) ");
            var answer = ReadLine()?.Trim().ToUpperInvariant();

            Game game = null;

            switch (answer)
            {
                case "O":
                    try
                    {
                        // Lister toutes les parties non terminées
                        var inProgress = stubGames.Where(g => !g.IsFinished).ToList();
                        if (!inProgress.Any())
                        {
                            WriteLine("Aucune partie en cours disponible.");
                            game = Utils.CreateNewGame();
                        }
                        else
                        {
                            WriteLine("Parties disponibles (Code – Joueurs) :");
                            foreach (var g in inProgress)
                                WriteLine($"{g.Id} – {string.Join(", ", g.Players.Select(p => p.Name))}");

                            // Saisie du code (5 caractères)
                            Write("Entrez le code de la partie à reprendre : ");
                            var codeInput = (ReadLine() ?? "")
                                                .Trim()
                                                .ToUpperInvariant();

                            // Recherche exacte
                            var resumed = inProgress
                                .FirstOrDefault(g => string.Equals(g.Id, codeInput, StringComparison.OrdinalIgnoreCase));

                            if (resumed != null)
                            {
                                game = resumed;
                                Game.RaiseGameResumed(game);
                            }
                            else
                            {
                                throw new Error(ErrorCodes.GameIdNotFound);
                            }
                        }
                    }
                    catch (Error e)
                    {
                        // Erreur de reprise -> message + nouvelle partie
                        var errorHandler = new ErrorHandler(e);
                        Utils.WriteError(errorHandler.Handle());
                        WriteLine("Appuyez sur une touche pour continuer...");
                        ReadKey(true);
                        game = Utils.CreateNewGame();
                    }
                    break;

                case "N":
                    // Nouvelle partie
                    game = Utils.CreateNewGame();
                    break;

                default:
                    // Choix invalide -> message + nouvelle partie
                    var handler = new ErrorHandler(new Error(ErrorCodes.InvalidChoice));
                    Utils.WriteError(handler.Handle());
                    WriteLine("Appuyez sur une touche pour continuer...");
                    ReadKey(true);
                    game = Utils.CreateNewGame();
                    break;
            }

            // 3) Souscrire aux événements de la partie
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
                Utils.WriteGameMaster("Position (ligne,colonne) – ex : 1,1");
                var src = ReadLine()!;
                Utils.WriteGameMaster("Carte à recouvrir (ligne,colonne) – ex : 1,2");
                var dst = ReadLine()!;
                game.HandlePlayerChooseCover(e.Player,
                    Utils.ParsePosition(src),
                    Utils.ParsePosition(dst));
            };
            game.PlayerChooseDuck += (s, e) =>
            {
                Utils.WriteGameMaster("Quelle carte déplacer ? (ligne,colonne) – ex : 1,1");
                var src = ReadLine()!;
                Utils.WriteGameMaster("Destination (ligne,colonne) – ex : 2,3");
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

            // 4) Lancer la boucle de jeu
            game.GameLoop();
        }
    }
}
