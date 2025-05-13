using System;
using System.Collections.Generic;
using Model;
using static System.Console;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string Title = @"
______            _     ___            _ _____                     
|  _  \          | |   / _ \          | /  __ \                    
| | | |_   _  ___| | _/ /_\ \_ __   __| | /  \/ _____   _____ _ __ 
| | | | | | |/ __| |/ /  _  | '_ \ / _` | |    / _ \ \ / / _ \ '__|
| |/ /| |_| | (__|   <| | | | | | | (_| | \__/\ (_) \ V /  __/ |   
|___/  \__,_|\___|_|\_\_| |_|_| |_|\__,_|\____/\___/ \_/ \___|_|   
                                                                   
                                                                   
";
            ConsoleColor gameManagerColor = ConsoleColor.Cyan;

            Utils.WriteGameMaster(Title);
            Utils.WriteGameMaster("Combien de joueurs ?");

            string input = ReadLine();
            int nbJoueur;

            if (int.TryParse(input, out nbJoueur))
            {
                List<Player> players = new List<Player>();
                while (players.Count < nbJoueur)
                {
                    Utils.WriteGameMaster($"Pseudo du joueur numéro {players.Count + 1}:");
                    string playerName = ReadLine();
                    while (string.IsNullOrEmpty(playerName))
                    {
                        Utils.WriteGameMaster("Pseudo invalide, changez le pseudo.");
                        playerName = ReadLine();
                    }

                    Player player = new Player(playerName);
                    players.Add(player);
                    Utils.WriteGameMaster($"Joueur {playerName} ajouté avec succès!");
                }

                Game game = new Game(players);
                DeckGenerator deckGenerator = new DeckGenerator();
                deckGenerator.Generate();

                bool exitGame = false;
                int currentPlayerIndex = 0;
                int? lastNumber = null;

                while (!exitGame)
                {
                    if (deckGenerator.Deck.Count == 0)
                    {
                        Utils.WriteGameMaster("Le deck est vide. La partie est terminée !");
                        exitGame = true;
                        break;
                    }

                    DeckCard currentDeckCard = deckGenerator.Deck[0];
                    Player currentPlayer = players[currentPlayerIndex];

                    if (currentDeckCard.Bonus == Bonus.Again && lastNumber.HasValue)
                    {
                        currentDeckCard.Number = lastNumber.Value;
                        Utils.WriteGameMaster($"Carte Again active ! Le numéro utilisé est {currentDeckCard.Number}");
                    }
                    else if (currentDeckCard.Bonus == Bonus.Max)
                    {
                        int maxNumber = currentPlayer.Grid.GameCardsGrid.Max(c => c.Number);
                        currentDeckCard.Number = maxNumber;
                        Utils.WriteGameMaster($"Carte MAX ! Le numéro utilisé est {maxNumber} (le plus grand de la grille de {currentPlayer.Name})");
                    }
                    else
                    {
                        lastNumber = currentDeckCard.Number;
                        Utils.WriteGameMaster($"Carte actuelle du deck : {(currentDeckCard.Number == 0 ? currentDeckCard.Bonus.ToString() : currentDeckCard.Number.ToString())}");
                    }

                    bool allPassed = players.All(p => p.HasPassed);
                    if (allPassed)
                    {
                        Utils.WriteGameMaster("Tous les joueurs ont passé leur tour. Carte défaussée.");
                        deckGenerator.Deck.RemoveAt(0);
                        game.CardPassed++;
                        foreach (var p in players)
                            p.HasPassed = false;
                        continue;
                    }

                    if (game.Rules.IsGameOver(game.CardPassed, players[currentPlayerIndex].StackCounter))
                    {
                        Utils.WriteGameMaster("La partie est terminée !");
                        Utils.DisplayPlayerScores(players);
                        exitGame = true;
                        break;
                    }

                    Utils.WriteGameMaster($"\nC'est au tour de {currentPlayer.Name}");
                    Utils.DisplayGrid(currentPlayer);
                    Utils.WriteGameMaster("Que souhaitez-vous faire ?");
                    Utils.DisplayMenu();

                    Write("\nVotre choix: ");
                    string choice = ReadLine();

                    switch (choice)
                    {
                        case "1":
                            PerformCoverAction(currentPlayer, game, ref currentPlayerIndex, players, currentDeckCard);
                            break;
                        case "2":
                            PerformDuckAction(currentPlayer, game, ref currentPlayerIndex, players);
                            break;
                        case "3":
                            Utils.WriteGameMaster($"{currentPlayer.Name} dit : Coin ! Je n'ai pas de carte à jouer.");
                            currentPlayer.CallCoin(game);
                            currentPlayer.HasPassed = true;
                            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                            break;
                        case "4":
                            foreach (Player player in players)
                            {
                                Utils.WriteGameMaster($"Grille de {player.Name}:");
                                Utils.DisplayGrid(player);
                            }
                            break;
                        case "5":
                            Utils.DisplayPlayerScores(players);
                            break;
                        case "6":
                            exitGame = true;
                            Utils.WriteGameMaster("Merci d'avoir joué à Duck&Cover!");
                            break;
                        default:
                            Utils.WriteGameMaster("Choix invalide. Veuillez réessayer.");
                            break;
                    }
                }
            }
            else
            {
                Utils.WriteGameMaster("Nombre de joueurs invalide. Veuillez redémarrer le jeu.");
            }
        }

        static void PerformCoverAction(Player player, Game game, ref int currentPlayerIndex, List<Player> players,DeckCard currentDeckCard)
        {
            Utils.WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir?");
            Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
            string fromPosition = ReadLine();
            
            Utils.WriteGameMaster("Quelle carte souhaitez-vous recouvrir?");
            Utils.WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,2");
            string toPosition = ReadLine();
            
            try
            {
                var fromPos = Utils.ParsePosition(fromPosition);
                var toPos = Utils.ParsePosition(toPosition);

                GameCard fromCard = player.Grid.GetCard(fromPos);
                GameCard toCard = player.Grid.GetCard(toPos);
                if (!game.Rules.isTheSameCard(fromCard,currentDeckCard))
                {
                    Utils.WriteGameMaster("Impossible de jouer cette carte car ce n'est pas la carte actuelle");
                    return;
                }

                if (fromCard == null || toCard == null)
                {
                    Utils.WriteGameMaster("Une des positions ne contient pas de carte!");
                    return;
                }
                
                Utils.WriteGameMaster($"Tentative de recouvrir la carte {toCard.Number} (splash {toCard.Splash}) " +
                              $"avec la carte {fromCard.Number} (splash {fromCard.Splash})");
                
                bool success = player.Cover(fromCard, toCard, player.Grid, game);

                
                
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
            string fromPosition = ReadLine();
            
            Utils.WriteGameMaster("Où souhaitez-vous la déplacer?");
            Utils.WriteGameMaster("Entrez la nouvelle position (ligne,colonne) - exemple: 2,3");
            string toPosition = ReadLine();
            
            try
            {
                var fromPos = Utils.ParsePosition(fromPosition);
                var toPos = Utils.ParsePosition(toPosition);
                
                GameCard card = player.Grid.GetCard(fromPos);
                
                if (card == null)
                {
                    Utils.WriteGameMaster("Il n'y a pas de carte à cette position!");
                    return;
                }
                
                Utils.WriteGameMaster($"Tentative de déplacement de la carte {card.Number} (splash {card.Splash}) " +
                              $"de la position ({fromPos.Row}, {fromPos.Column}) " +
                              $"vers la position ({toPos.Row}, {toPos.Column}).");
                
                bool success = player.Duck(card, toPos, player.Grid, game);
                
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