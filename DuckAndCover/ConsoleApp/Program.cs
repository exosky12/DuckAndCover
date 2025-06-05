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
            game.PlayerChanged += (s, e) => HandlePlayerTurn(game, e.CurrentPlayer, e.CurrentDeckCard);
            game.DisplayMenuNeeded += (s, e) => HandlePlayerTurn(game, e.CurrentPlayer, e.CurrentDeckCard);
            game.PlayerChooseCoin += (s, e) => Utils.WriteGameMaster($"{e.Player.Name} a fait coin !");
            game.PlayerChooseCover += (s, e) => HandleCoverChoice(game, e.Player);
            game.PlayerChooseDuck += (s, e) => HandleDuckChoice(game, e.Player);
            game.PlayerChooseShowPlayersGrid += (s, e) => DisplayPlayerGrids(e.Players);
            game.PlayerChooseShowScores += (s, e) => Utils.DisplayPlayerScores(e.Players);
            game.PlayerChooseQuit += (s, e) => HandlePlayerQuit(game, e.Player);
            game.GameIsOver += (s, e) => HandleGameOver(game);
            game.ErrorOccurred += (s, e) => HandleError(e.ErrorException);
        }

        private static void HandlePlayerTurn(Game game, Player currentPlayer, DeckCard currentDeckCard)
        {
            while (true)
            {
                var choice = Utils.PromptPlayerTurn(currentPlayer, currentDeckCard, game);
                
                if (!IsValidChoice(choice))
                {
                    ShowErrorAndWait("Choix invalide. Veuillez réessayer.");
                    continue;
                }

                try
                {
                    game.HandlePlayerChoice(currentPlayer, choice);
                    break;
                }
                catch (ErrorException)
                {
                    ShowErrorAndWait("Une erreur s'est produite lors du traitement de votre choix.");
                }
            }
        }

        private static void HandleCoverChoice(Game game, Player player)
        {
            while (true)
            {
                try
                {
                    Utils.WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir ? (ex : A1)");
                    var src = ReadLine()!;
                    Utils.WriteGameMaster("Quelle carte souhaitez-vous recouvrir ? (ex : B2)");
                    var dst = ReadLine()!;
                    game.HandlePlayerChooseCover(player, Utils.ParsePosition(src), Utils.ParsePosition(dst));
                    break;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private static void HandleDuckChoice(Game game, Player player)
        {
            while (true)
            {
                try
                {
                    Utils.WriteGameMaster("Quelle carte souhaitez-vous déplacer ? (format : ligne, colonnes ex : 1,1)");
                    var src = ReadLine()!;
                    Utils.WriteGameMaster("Où souhaitez-vous la déplacer ? (format : ligne, colonne ex : 1,5)");
                    var dst = ReadLine()!;
                    game.HandlePlayerChooseDuck(player, Utils.ParsePosition(src), Utils.ParsePosition(dst));
                    break;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private static void DisplayPlayerGrids(IEnumerable<Player> players)
        {
            Utils.WriteGameMaster("Voici les grilles des joueurs :");
            foreach (var p in players)
            {
                Utils.WriteGameMaster($"Grille de {p.Name} :");
                Utils.DisplayGrid(p);
            }
        }

        private static void HandlePlayerQuit(Game game, Player player)
        {
            Utils.WriteGameMaster($"{player.Name} a quitté la partie.");
            game.Quit = true;
        }

        private static void HandleGameOver(Game game)
        {
            game.IsFinished = true;
            Utils.EndGame(game.Players, game);
        }

        private static void HandleError(ErrorException errorException)
        {
            var handler = new ErrorHandler(errorException);
            Utils.WriteError(handler.Handle());
            ShowErrorAndWait();
        }

        private static void HandleException(Exception ex)
        {
            ErrorHandler handler = ex is ErrorException errorEx 
                ? new ErrorHandler(errorEx)
                : new ErrorHandler(new ErrorException(ErrorCodes.UnknownError));
            Utils.WriteError(handler.Handle());
            ShowErrorAndWait();
        }

        private static void ShowErrorAndWait(string? message = null)
        {
            if (message != null)
                Utils.WriteError(message);
            WriteLine("Appuyez sur une touche pour continuer…");
            ReadKey(true);
        }

        private static bool IsValidChoice(string choice)
        {
            return choice == "1" || choice == "2" || choice == "3" || choice == "4" || choice == "5" || choice == "6";
        }
    }
}
