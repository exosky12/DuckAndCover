using Model;
using static System.Console;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            ShowTitle();
            int nbJoueur = AskNumberOfPlayers();
            if (nbJoueur <= 0)
            {
                Utils.WriteGameMaster("Nombre de joueurs invalide. Veuillez redémarrer le jeu.");
                return;
            }

            List<Player> players = InitializePlayers(nbJoueur);
            Game game = new Game(players);
            DeckGenerator deckGenerator = new DeckGenerator();
            deckGenerator.Generate();

            RunGameLoop(players, game, deckGenerator);
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

        static int AskNumberOfPlayers()
        {
            Utils.WriteGameMaster("Combien de joueurs ?");
            if (int.TryParse(ReadLine()!, out int nbJoueur)) return nbJoueur;
            return -1;
        }

        static List<Player> InitializePlayers(int count)
        {
            var players = new List<Player>();
            while (players.Count < count)
            {
                Utils.WriteGameMaster($"Pseudo du joueur numéro {players.Count + 1}:");
                string name = ReadLine()!;
                while (string.IsNullOrWhiteSpace(name))
                {
                    Utils.WriteGameMaster("Pseudo invalide, changez le pseudo.");
                    name = ReadLine()!;
                }

                players.Add(new Player(name));
                Utils.WriteGameMaster($"Joueur {name} ajouté avec succès!");
            }

            return players;
        }

        static void RunGameLoop(List<Player> players, Game game, DeckGenerator deckGenerator)
        {
            bool exitGame = false;
            int currentPlayerIndex = 0;
            int? lastNumber = null;

            while (!exitGame)
            {
                if (deckGenerator.Deck.Count == 0)
                {
                    Utils.WriteGameMaster("Le deck est vide. La partie est terminée !");
                    break;
                }

                DeckCard card = deckGenerator.Deck[0];
                Player currentPlayer = players[currentPlayerIndex];

                ProcessCardEffect(card, currentPlayer, ref lastNumber);

                if (AllPlayersPassed(players))
                {
                    HandleAllPassed(deckGenerator, game, players);
                    continue;
                }

                if (game.Rules.IsGameOver(game.CardPassed, currentPlayer.StackCounter))
                {
                    EndGame(players);
                    break;
                }

                PromptPlayerTurn(currentPlayer);
                string choice = ReadLine()!;
                exitGame = HandlePlayerChoice(choice, currentPlayer, game, ref currentPlayerIndex, players, card);
            }
        }

        static void ProcessCardEffect(DeckCard card, Player player, ref int? lastNumber)
        {
            switch (card.Bonus)
            {
                case Bonus.Again when lastNumber.HasValue:
                    card.Number = lastNumber.Value;
                    Utils.WriteGameMaster($"Carte Again active ! Le numéro utilisé est {card.Number}");
                    break;
                case Bonus.Max:
                    int max = player.Grid.GameCardsGrid.Max(c => c.Number);
                    card.Number = max;
                    Utils.WriteGameMaster($"Carte MAX ! Numéro utilisé : {max} (grille de {player.Name})");
                    break;
                default:
                    lastNumber = card.Number;
                    string msg = card.Number == 0 ? card.Bonus.ToString() : card.Number.ToString();
                    Utils.WriteGameMaster($"Carte actuelle du deck : {msg}");
                    break;
            }
        }

        static bool AllPlayersPassed(List<Player> players) => players.All(p => p.HasPassed);

        static void HandleAllPassed(DeckGenerator deckGenerator, Game game, List<Player> players)
        {
            Utils.WriteGameMaster("Tous les joueurs ont passé leur tour. Carte défaussée.");
            deckGenerator.Deck.RemoveAt(0);
            game.CardPassed++;
            players.ForEach(p => p.HasPassed = false);
        }

        static void EndGame(List<Player> players)
        {
            Utils.WriteGameMaster("La partie est terminée !");
            Utils.DisplayPlayerScores(players);
        }

        static void PromptPlayerTurn(Player player)
        {
            Utils.WriteGameMaster($"\nC'est au tour de {player.Name}");
            Utils.DisplayGrid(player);
            Utils.WriteGameMaster("Que souhaitez-vous faire ?");
            Utils.DisplayMenu();
            Write("\nVotre choix: ");
        }

        static bool HandlePlayerChoice(string choice, Player player, Game game, ref int index, List<Player> players,
            DeckCard card)
        {
            switch (choice)
            {
                case "1":
                    PerformCoverAction(player, game, ref index, players, card);
                    break;
                case "2":
                    PerformDuckAction(player, game, ref index, players);
                    break;
                case "3":
                    Utils.WriteGameMaster($"{player.Name} dit : Coin !");
                    Player.CallCoin(game);
                    player.HasPassed = true;
                    index = (index + 1) % players.Count;
                    break;
                case "4":
                    players.ForEach(p =>
                    {
                        Utils.WriteGameMaster($"Grille de {p.Name}:");
                        Utils.DisplayGrid(p);
                    });
                    break;
                case "5":
                    Utils.DisplayPlayerScores(players);
                    break;
                case "6":
                    Utils.WriteGameMaster("Merci d'avoir joué à Duck&Cover!");
                    return true;
                default:
                    Utils.WriteGameMaster("Choix invalide. Veuillez réessayer.");
                    break;
            }

            return false;
        }

        static void PerformCoverAction(Player player, Game game, ref int currentPlayerIndex, List<Player> players,
            DeckCard currentDeckCard)
        {
            Utils.WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir?");
            Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
            string fromPosition = ReadLine()!;

            Utils.WriteGameMaster("Quelle carte souhaitez-vous recouvrir?");
            Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,2");
            string toPosition = ReadLine()!;

            try
            {
                var fromPos = Utils.ParsePosition(fromPosition);
                var toPos = Utils.ParsePosition(toPosition);

                GameCard? fromCard = player.Grid.GetCard(fromPos);
                GameCard? toCard = player.Grid.GetCard(toPos);
                
                if (fromCard == null || toCard == null)
                {
                    Utils.WriteGameMaster("Une des positions ne contient pas de carte!");
                    return;
                }
                
                if (!game.Rules.isTheSameCard(fromCard, currentDeckCard))
                {
                    Utils.WriteGameMaster("Impossible de jouer cette carte car ce n'est pas la carte actuelle");
                    return;
                }


                Utils.WriteGameMaster($"Tentative de recouvrir la carte {toCard.Number} (splash {toCard.Splash}) " +
                                      $"avec la carte {fromCard.Number} (splash {fromCard.Splash})");

                bool success = Player.Cover(fromCard, toCard, player.Grid, game);


                if (success)
                {
                    Utils.WriteGameMaster("Recouvrement réussi!");
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                }
                else
                {
                    Utils.WriteGameMaster("Recouvrement impossible avec ces cartes.");
                }
            }
            catch (Exception e)
            {
                Utils.WriteGameMaster($"Erreur: {e.Message}");
            }
        }

        static void PerformDuckAction(Player player, Game game, ref int currentPlayerIndex, List<Player> players)
        {
            Utils.WriteGameMaster("Quelle carte souhaitez-vous déplacer?");
            Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
            string fromPosition = ReadLine()!;

            Utils.WriteGameMaster("Où souhaitez-vous la déplacer?");
            Utils.WriteGameMaster("Entrez la nouvelle position (ligne,colonne) - exemple: 2,3");
            string toPosition = ReadLine()!;

            try
            {
                var fromPos = Utils.ParsePosition(fromPosition);
                var toPos = Utils.ParsePosition(toPosition);

                GameCard? card = player.Grid.GetCard(fromPos);

                if (card == null)
                {
                    Utils.WriteGameMaster("Il n'y a pas de carte à cette position!");
                    return;
                }

                Utils.WriteGameMaster($"Tentative de déplacement de la carte {card.Number} (splash {card.Splash}) " +
                                      $"de la position ({fromPos.Row}, {fromPos.Column}) " +
                                      $"vers la position ({toPos.Row}, {toPos.Column}).");

                bool success = Player.Duck(card, toPos, player.Grid, game);

                if (success)
                {
                    Utils.WriteGameMaster("Déplacement réussi!");
                    // Passer au joueur suivant après une action réussie
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                }
                else
                {
                    Utils.WriteGameMaster("Déplacement impossible vers cette position.");
                }
            }
            catch (Exception e)
            {
                Utils.WriteGameMaster($"Erreur: {e.Message}");
            }
        }
    }
}