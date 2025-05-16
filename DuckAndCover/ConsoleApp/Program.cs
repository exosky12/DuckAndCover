using Models.Game;
using Models.Events;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main()
        {
            ShowTitle();
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
                string choice = Utils.PromptPlayerTurn(args.CurrentPlayer, args.DeckCard, game);
                game.SubmitChoice(choice);
            };


            game.GameIsOver += (sender, args) =>
            {
                Utils.EndGame(game.Players, game);
            };

            game.GameLoop();
        }

        static void ShowTitle()
        {
            string title = @"
______            _     ___            _ _____                     
|  _  \          | |   / _ \          | /  __ \                    
| | | |_   _  ___| | _/ /_\ \_ __   __| | /  \/ _____   _____ _ __ 
| | | | | | |/ __| |/ /  _  | '_ \ / _` | |    / _ \ \ / / _ \ '__|
| |/ /| |_| | (__|   <| | | | | | | (_| | \__/\ (_) \ V /  __/ |   
|___/  \__,_|\___|_|\_\_| |_|_| |_|\__,_|\____/\___/ \_/ \___|_|   
                                                                   
                                                                   
";
            Utils.WriteGameMaster(title);
        }
    }
}