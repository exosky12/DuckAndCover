using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using static System.Console;
using Models.Game;
using Models.Exceptions;
using Models.Rules;
using DataPersistence;
using Models.Enums;
using Models.Interfaces;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main()
        {
            Utils.ShowTitle();

            var stub = new FakePersistency();
            var (_, stubGames) = stub.LoadData();

            Game game = GetGameFromUserChoice(stubGames);

            if (game == null)
                throw new ErrorException(ErrorCodes.UnknownError);

            SubscribeToGameEvents(game);

            game.StartGame();
        }

        private static Game GetGameFromUserChoice(ObservableCollection<Game> stubGames)
        {
            Game? game = null;

            while (true)
            {
                Write("Souhaitez-vous reprendre une partie en cours ? (O/N) ");
                var answer = ReadLine()?.Trim().ToUpperInvariant();

                switch (answer)
                {
                    case "O":
                        ResumeGame(stubGames, out game);
                        return game!;

                    case "N":

                        IRules rules = Utils.ChoisirRegles();
                        int count;
                        while (true)
                        {
                            Utils.WriteGameMaster("Nombre de joueurs (2 à 7) :");
                            if (!int.TryParse(ReadLine(), out count) || count < 2 || count > 7)
                            {
                                var handler2 = new ErrorHandler(new ErrorException(ErrorCodes.InvalidChoice));
                                Utils.WriteError(handler2.Handle());
                                WriteLine("Appuyez sur une touche pour recommencer…");
                                ReadKey(true);
                                Utils.ClearCurrentConsoleLine();
                                continue;
                            }
                            break;
                        }

                        var players = Utils.InitializePlayers(count);

                        game = new Game(rules);

                        Deck deck = new Deck();
                        if (deck.Cards.Count == 0)
                            throw new ErrorException(ErrorCodes.DeckEmpty);

                        string newId = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpperInvariant();
                        game.InitializeGame(
                            newId,
                            players,
                            deck,
                            deck.Cards.First()
                        );

                        return game!;

                    default:
                        var handler3 = new ErrorHandler(new ErrorException(ErrorCodes.InvalidChoice));
                        Utils.WriteError(handler3.Handle());
                        WriteLine("Appuyez sur une touche pour recommencer…");
                        ReadKey(true);
                        Utils.ClearCurrentConsoleLine();
                        break;
                }
            }
        }

        private static void ResumeGame(ObservableCollection<Game> stubGames, out Game game)
        {
            var inProgress = stubGames.Where(g => !g.IsFinished).ToList();

            if (inProgress.Count == 0)
            {
                Utils.WriteGameMaster("Aucune partie en cours disponible. Création d'une nouvelle partie Classic par défaut.");
                game = CreateDefaultGame();
                return;
            }

            while (true)
            {
                WriteLine();
                WriteLine("Parties en cours (Code – Joueurs) :");
                foreach (var g in inProgress)
                    WriteLine($"  {g.Id} – {string.Join(", ", g.Players.Select(p => p.Name))}");

                Write("Entrez le code de la partie à reprendre : ");
                var codeInput = (ReadLine() ?? "").Trim().ToUpperInvariant();

                var resumed = inProgress.FirstOrDefault(g =>
                    g.Id.Equals(codeInput, StringComparison.OrdinalIgnoreCase));

                if (resumed != null)
                {
                    game = resumed;
                    return;
                }

                var handler = new ErrorHandler(new ErrorException(ErrorCodes.GameIdNotFound));
                Utils.WriteError(handler.Handle());
                WriteLine("Appuyez sur une touche pour continuer…");
                ReadKey(true);
                Utils.ClearCurrentConsoleLine();
            }
        }

        private static Game CreateDefaultGame()
        {
            IRules rules = new ClassicRules();
            int count;
            while (true)
            {
                Utils.WriteGameMaster("Nombre de joueurs (2 à 7) pour la partie Classic :");
                if (!int.TryParse(ReadLine(), out count) || count < 2 || count > 7)
                {
                    var handler = new ErrorHandler(new ErrorException(ErrorCodes.InvalidChoice));
                    Utils.WriteError(handler.Handle());
                    WriteLine("Appuyez sur une touche pour recommencer…");
                    ReadKey(true);
                    Utils.ClearCurrentConsoleLine();
                    continue;
                }
                break;
            }

            var players = Utils.InitializePlayers(count);

            Game game = new Game(rules);

            Deck deck = new Deck();
            if (deck.Cards.Count == 0)
                throw new ErrorException(ErrorCodes.DeckEmpty);

            string newId = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpperInvariant();
            game.InitializeGame(
                newId,
                players,
                deck,
                deck.Cards.First()
            );
            return game;
        }

        private static void SubscribeToGameEvents(Game game)
        {
            game.PlayerChanged += (s, e) =>
            {
                while (true)
                {
                    var choice = Utils.PromptPlayerTurn(e.CurrentPlayer, e.CurrentDeckCard, game);
                    
                    if (IsValidChoice(choice))
                    {
                        try
                        {
                            game.HandlePlayerChoice(e.CurrentPlayer, choice);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteError("Une erreur s'est produite lors du traitement de votre choix.");
                            WriteLine("Appuyez sur une touche pour continuer…");
                            ReadKey(true);
                        }
                    }
                    else
                    {
                        Utils.WriteError("Choix invalide. Veuillez réessayer.");
                        WriteLine("Appuyez sur une touche pour continuer…");
                        ReadKey(true);
                    }
                }
            };

            game.DisplayMenuNeeded += (s, e) =>
            {
                while (true)
                {
                    var choice = Utils.PromptPlayerTurn(e.CurrentPlayer, e.CurrentDeckCard, game);
                    
                    if (IsValidChoice(choice))
                    {
                        try
                        {
                            game.HandlePlayerChoice(e.CurrentPlayer, choice);
                            break; 
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteError("Une erreur s'est produite lors du traitement de votre choix.");
                            WriteLine("Appuyez sur une touche pour continuer…");
                            ReadKey(true);
                        }
                    }
                    else
                    {
                        Utils.WriteError("Choix invalide. Veuillez réessayer.");
                        WriteLine("Appuyez sur une touche pour continuer…");
                        ReadKey(true);
                    }
                }
            };

            game.PlayerChooseCoin += (s, e) =>
                Utils.WriteGameMaster($"{e.Player.Name} a fait coin !");

            game.PlayerChooseCover += (s, e) =>
            {
                while (true)
                {
                    try
                    {
                        Utils.WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir ? (ex : A1)");
                        var src = ReadLine()!;
                        Utils.WriteGameMaster("Quelle carte souhaitez-vous recouvrir ? (ex : B2)");
                        var dst = ReadLine()!;
                        game.HandlePlayerChooseCover(
                            e.Player,
                            Utils.ParsePosition(src),
                            Utils.ParsePosition(dst)
                        );
                        break;
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler handler;
                        if (ex is ErrorException errorEx)
                        {
                            handler = new ErrorHandler(errorEx);
                        }
                        else
                        {
                            handler = new ErrorHandler(new ErrorException(ErrorCodes.UnknownError));
                        }
                        Utils.WriteError(handler.Handle());
                        WriteLine("Appuyez sur une touche pour continuer…");
                        ReadKey(true);
                    }
                }
            };

            game.PlayerChooseDuck += (s, e) =>
            {
                while (true)
                {
                    try
                    {
                        Utils.WriteGameMaster("Quelle carte souhaitez-vous déplacer ? (ex : C3)");
                        var src = ReadLine()!;
                        Utils.WriteGameMaster("Où souhaitez-vous la déplacer ? (ex : D4)");
                        var dst = ReadLine()!;
                        game.HandlePlayerChooseDuck(
                            e.Player,
                            Utils.ParsePosition(src),
                            Utils.ParsePosition(dst)
                        );
                        break;
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler handler;
                        if (ex is ErrorException errorEx)
                        {
                            handler = new ErrorHandler(errorEx);
                        }
                        else
                        {
                            handler = new ErrorHandler(new ErrorException(ErrorCodes.UnknownError));
                        }
                        Utils.WriteError(handler.Handle());
                        WriteLine("Appuyez sur une touche pour continuer…");
                        ReadKey(true);
                    }
                }
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
            {
                Utils.DisplayPlayerScores(e.Players);
            };

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
                var handler = new ErrorHandler(e.ErrorException);
                Utils.WriteError(handler.Handle());
                WriteLine("Appuyez sur une touche pour continuer…");
                ReadKey(true);
            };
        }

        private static bool IsValidChoice(string choice)
        {
            return choice == "1" || choice == "2" || choice == "3" || choice == "4" || choice == "5" || choice == "6";
        }
    }
}
