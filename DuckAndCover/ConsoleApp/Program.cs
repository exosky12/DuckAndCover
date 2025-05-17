using Models.Game;
using Models.Exceptions;
using static System.Console;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main()
        {
            Utils.ShowTitle();
            int nbJoueur = Utils.AskNumberOfPlayers();
            if (nbJoueur <= 0)
            {
                Utils.WriteGameMaster("Nombre de joueurs invalide. Veuillez redémarrer le jeu.");
                return;
            }

            List<Player> players = Utils.InitializePlayers(nbJoueur);
            Game game = new Game(players);
            
            game.PlayerChanged += (sender, args) =>
            {
                string choice = Utils.PromptPlayerTurn(args.CurrentPlayer, args.DeckCard, sender as Game);
                game.SubmitChoice(choice);
            };

            game.DisplayMenuNeeded += (sender, args) =>
            {
                string choice = Utils.PromptPlayerTurn(args.CurrentPlayer, args.DeckCard, sender as Game);
                game.SubmitChoice(choice);
            };

            game.PlayerChooseCoin += (sender, args) => { Utils.WriteGameMaster($"{args.Player.Name} a fait coin !"); };

            game.PlayerChooseCover += (sender, args) =>
            {
                Utils.WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir?");
                Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
                string cardToMovePosition = ReadLine()!;

                Utils.WriteGameMaster("Quelle carte souhaitez-vous recouvrir?");
                Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,2");
                string cardToCover = ReadLine()!;

                game.HandlePlayerChooseCover(args.Player, Utils.ParsePosition(cardToMovePosition),
                    Utils.ParsePosition(cardToCover));
            };

            game.PlayerChooseDuck += (sender, args) =>
            {
                Utils.WriteGameMaster("Quelle carte souhaitez-vous déplacer?");
                Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
                string cardToMovePosition = ReadLine()!;

                Utils.WriteGameMaster("Où souhaitez-vous la déplacer?");
                Utils.WriteGameMaster("Entrez la nouvelle position (ligne,colonne) - exemple: 2,3");
                string duckPosition = ReadLine()!;

                game.HandlePlayerChooseDuck(args.Player, Utils.ParsePosition(cardToMovePosition),
                    Utils.ParsePosition(duckPosition));
            };
            

            game.PlayerChooseShowPlayersGrid += (sender, args) =>
            {
                Utils.WriteGameMaster("Voici la grille des joueurs :");
                foreach (var player in args.Players)
                {
                    Utils.WriteGameMaster($"Grille de {player.Name} :");
                    Utils.DisplayGrid(player);
                }
            };

            game.PlayerChooseShowScores += (sender, args) =>
            {
                Utils.WriteGameMaster("Voici les scores des joueurs :");
                Utils.DisplayPlayerScores(args.Players);
            };

            game.PlayerChooseQuit += (sender, args) =>
            {
                Utils.WriteGameMaster($"{args.Player.Name} a quitté la partie.");
                game.Quit = true;
            };

            game.GameIsOver += (sender, args) => { Utils.EndGame(game.Players, sender as Game); };
            
            game.ErrorOccurred += (sender, args) =>
            {
                var errorHandler = new ErrorHandler(args.Error);
                Utils.WriteError(errorHandler.Handle());
                WriteLine("Appuyez sur une touche pour continuer...");
                ReadKey(true);
            };
            
            game.GameLoop();
        }
    }
}