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

            WriteGameMaster(Title);
            WriteGameMaster("Combien de joueurs ?");

            string input = ReadLine();
            int nbJoueur;

            if (int.TryParse(input, out nbJoueur))
            {
                List<Player> players = new List<Player>();
                while (players.Count < nbJoueur)
                {
                    WriteGameMaster($"Pseudo du joueur numéro {players.Count + 1}:");
                    string playerName = ReadLine();
                    while (string.IsNullOrEmpty(playerName))
                    {
                        WriteGameMaster("Pseudo invalide, changez le pseudo.");
                        playerName = ReadLine();
                    }

                    Player player = new Player(playerName);
                    players.Add(player);
                    WriteGameMaster($"Joueur {playerName} ajouté avec succès!");
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
                        WriteGameMaster("Le deck est vide. La partie est terminée !");
                        exitGame = true;
                        break;
                    }

                    DeckCard currentDeckCard = deckGenerator.Deck[0];
                    Player currentPlayer = players[currentPlayerIndex];

                    if (currentDeckCard.Bonus == Bonus.Again && lastNumber.HasValue)
                    {
                        currentDeckCard.Number = lastNumber.Value;
                        WriteGameMaster($"Carte Again active ! Le numéro utilisé est {currentDeckCard.Number}");
                    }
                    else if (currentDeckCard.Bonus == Bonus.Max)
                    {
                        int maxNumber = currentPlayer.Grid.GameCardsGrid.Max(c => c.Number);
                        currentDeckCard.Number = maxNumber;
                        WriteGameMaster($"Carte MAX ! Le numéro utilisé est {maxNumber} (le plus grand de la grille de {currentPlayer.Name})");
                    }
                    else
                    {
                        lastNumber = currentDeckCard.Number;
                        WriteGameMaster($"Carte actuelle du deck : {(currentDeckCard.Number == 0 ? currentDeckCard.Bonus.ToString() : currentDeckCard.Number.ToString())}");
                    }

                    bool allPassed = players.All(p => p.HasPassed);
                    if (allPassed)
                    {
                        WriteGameMaster("Tous les joueurs ont passé leur tour. Carte défaussée.");
                        deckGenerator.Deck.RemoveAt(0);
                        foreach (var p in players)
                            p.HasPassed = false;
                        continue;
                    }

                    if (game.Rules.IsGameOver(game.CardPassed, players[currentPlayerIndex].StackCounter))
                    {
                        WriteGameMaster("La partie est terminée !");
                        DisplayPlayerScores(players);
                        exitGame = true;
                        break;
                    }

                    WriteGameMaster($"\nC'est au tour de {currentPlayer.Name}");
                    DisplayGrid(currentPlayer.Grid);
                    WriteGameMaster("Que souhaitez-vous faire ?");
                    WriteLine("\n1. Cover (recouvrir une carte)");
                    WriteLine("2. Duck (déplacer une carte)");
                    WriteLine("3. Call Coin (passer son tour)");
                    WriteLine("4. Afficher les grilles de tous les joueurs");
                    WriteLine("5. Afficher les scores");
                    WriteLine("6. Quitter la partie");
                    WriteLine("7. Afficher tout le deck");

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
                            WriteGameMaster($"{currentPlayer.Name} dit : Coin ! Je n'ai pas de carte à jouer.");
                            currentPlayer.CallCoin(game);
                            currentPlayer.HasPassed = true;
                            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                            break;
                        case "4":
                            foreach (Player p in players)
                            {
                                WriteGameMaster($"Grille de {p.Name}:");
                                DisplayGrid(p.Grid);
                            }
                            break;
                        case "5":
                            DisplayPlayerScores(players);
                            break;
                        case "6":
                            exitGame = true;
                            WriteGameMaster("Merci d'avoir joué à Duck&Cover!");
                            break;
                        case "7":
                            DisplayFullDeck(deckGenerator.Deck);
                            break;
                        default:
                            WriteGameMaster("Choix invalide. Veuillez réessayer.");
                            break;
                    }
                }
            }
            else
            {
                WriteGameMaster("Nombre de joueurs invalide. Veuillez redémarrer le jeu.");
            }
        }


        static void DisplayFullDeck(List<DeckCard> deck)
        {
            WriteLine("\n╔═══════════════════════════ DECK COMPLET ═══════════════════════════╗");

            if (deck.Count == 0)
            {
                WriteLine("║                         Le deck est vide.                        ║");
                WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                return;
            }

            int count = 0;
            foreach (var card in deck)
            {
                string cardText = card.Bonus switch
                {
                    Bonus.Max => "MAX",
                    Bonus.Again => "AGAIN",
                    _ => card.Number.ToString("D2")
                };

                // Couleurs par bonus
                switch (card.Bonus)
                {
                    case Bonus.Max:
                        ForegroundColor = ConsoleColor.Blue;
                        break;
                    case Bonus.Again:
                        ForegroundColor = ConsoleColor.Green;
                        break;
                    default:
                        ForegroundColor = ConsoleColor.Gray;
                        break;
                }

                Write($"| {cardText,-6} ");
                ResetColor();

                count++;
                if (count % 6 == 0) WriteLine(); // Nouvelle ligne tous les 6
            }

            WriteLine("\n╚══════════════════════════════════════════════════════════════════╝");
        }


        static void WriteGameMaster(string message)
        {
            ConsoleColor originalColor = ForegroundColor;
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine($"\n[Maître du jeu] {message}");
            ForegroundColor = originalColor;
        }

        static void PerformCoverAction(Player player, Game game, ref int currentPlayerIndex, List<Player> players,DeckCard currentDeckCard)
        {
            WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir?");
            WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
            string fromPosition = ReadLine();
            
            WriteGameMaster("Quelle carte souhaitez-vous recouvrir?");
            WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,2");
            string toPosition = ReadLine();
            
            try
            {
                var fromPos = ParsePosition(fromPosition);
                var toPos = ParsePosition(toPosition);

                GameCard fromCard = player.Grid.GetCard(fromPos);
                GameCard toCard = player.Grid.GetCard(toPos);
                if (!game.Rules.isTheSameCard(fromCard,currentDeckCard))
                {
                    WriteGameMaster("Impossible de jouer cette carte car ce n'est pas la carte actuelle");
                    return;
                }

                if (fromCard == null || toCard == null)
                {
                    WriteGameMaster("Une des positions ne contient pas de carte!");
                    return;
                }
                
                WriteGameMaster($"Tentative de recouvrir la carte {toCard.Number} (splash {toCard.Splash}) " +
                              $"avec la carte {fromCard.Number} (splash {fromCard.Splash})");
                
                bool success = player.Cover(fromCard, toCard, player.Grid, game);

                
                
                if (success)
                {
                    WriteGameMaster("Recouvrement réussi!");
                    // Passer au joueur suivant après une action réussie
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                }
                else
                {
                    WriteGameMaster("Recouvrement impossible avec ces cartes.");
                }
            }
            catch (Exception e)
            {
                WriteGameMaster($"Erreur: {e.Message}");
            }
        }
        
        static void PerformDuckAction(Player player, Game game, ref int currentPlayerIndex, List<Player> players)
        {
            WriteGameMaster("Quelle carte souhaitez-vous déplacer?");
            WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
            string fromPosition = ReadLine();
            
            WriteGameMaster("Où souhaitez-vous la déplacer?");
            WriteGameMaster("Entrez la nouvelle position (ligne,colonne) - exemple: 2,3");
            string toPosition = ReadLine();
            
            try
            {
                var fromPos = ParsePosition(fromPosition);
                var toPos = ParsePosition(toPosition);
                
                GameCard card = player.Grid.GetCard(fromPos);
                
                if (card == null)
                {
                    WriteGameMaster("Il n'y a pas de carte à cette position!");
                    return;
                }
                
                WriteGameMaster($"Tentative de déplacement de la carte {card.Number} (splash {card.Splash}) " +
                              $"de la position ({fromPos.Row}, {fromPos.Column}) " +
                              $"vers la position ({toPos.Row}, {toPos.Column}).");
                
                bool success = player.Duck(card, toPos, player.Grid, game);
                
                if (success)
                {
                    WriteGameMaster("Déplacement réussi!");
                    // Passer au joueur suivant après une action réussie
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                }
                else
                {
                    WriteGameMaster("Déplacement impossible vers cette position.");
                }
            }
            catch (Exception e)
            {
                WriteGameMaster($"Erreur: {e.Message}");
            }
        }
        
        static Position ParsePosition(string input)
        {
            string[] parts = input.Split(',');
            if (parts.Length != 2)
                throw new ArgumentException("Format de position invalide. Utilisez le format: ligne,colonne");
                
            if (!int.TryParse(parts[0].Trim(), out int row) || !int.TryParse(parts[1].Trim(), out int col))
                throw new ArgumentException("Les valeurs de ligne et colonne doivent être numériques");
                
            return new Position(row, col);
        }

        static void DisplayPlayerScores(List<Player> players)
        {
            WriteGameMaster("\nScores actuels:");
            foreach (Player p in players)
            {
                WriteLine($"  {p.Name}: {p.GameScore} points");
            }
        }

        static void DisplayGrid(Grid grid)
        {
            WriteLine("\n╔════════════════════ GRILLE DE JEU ════════════════════╗");
            
            // Si la grille est vide, afficher un message
            if (grid.GameCardsGrid.Count == 0)
            {
                WriteLine("│                Grille actuellement vide                 │");
                WriteLine("╚══════════════════════════════════════════════════════╝");
                return;
            }

            var positions = new List<Position>();
            foreach (var card in grid.GameCardsGrid)
                positions.Add(card.Position);

            var (minX, maxX, minY, maxY) = GetBounds(positions);

            // Afficher les indices de colonnes
            Write("    ");
            for (int col = minX; col <= maxX; col++)
            {
                Write($"   {col}    ");
            }
            WriteLine();

            // Ligne de séparation supérieure
            Write("    ");
            for (int col = minX; col <= maxX; col++)
            {
                Write("────────");
            }
            WriteLine();

            for (int row = minY; row <= maxY; row++)
            {
                // Afficher l'indice de ligne
                Write($" {row} │");

                for (int col = minX; col <= maxX; col++)
                {
                    var card = grid.GetCard(new Position(row, col));
                    if (card != null)
                    {
                        SetSplashColor(card.Splash);
                        Write($" {card.Number:D2}-{card.Splash,-4}");
                        ResetColor();
                        Write("│");
                    }
                    else
                    {
                        Write($"       │");
                    }
                }

                WriteLine();

                // Ligne de séparation
                Write("    ");
                for (int col = minX; col <= maxX; col++)
                {
                    Write("────────");
                }
                WriteLine();
            }
            
            WriteLine("╚══════════════════════════════════════════════════════╝");
        }

        static void SetSplashColor(int splash)
        {
            if (splash == 0)
            {
                ForegroundColor = ConsoleColor.White;
            }
            else if (splash == 1)
            {
                ForegroundColor = ConsoleColor.Yellow;
            }
            else if (splash == 2)
            {
                ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (splash == 3)
            {
                ForegroundColor = ConsoleColor.Red;
            }
            else if (splash == 4)
            {
                ForegroundColor = ConsoleColor.DarkRed;
            }
            else
            {
                ForegroundColor = ConsoleColor.Magenta;
            }
        }

        static (int minX, int maxX, int minY, int maxY) GetBounds(List<Position> positions)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;

            foreach (var pos in positions)
            {
                if (pos.Column < minX) minX = pos.Column;
                if (pos.Column > maxX) maxX = pos.Column;
                if (pos.Row < minY) minY = pos.Row;
                if (pos.Row > maxY) maxY = pos.Row;
            }

            // Si aucune carte, définir des valeurs par défaut
            if (minX == int.MaxValue)
            {
                minX = 0;
                maxX = 0;
                minY = 0;
                maxY = 0;
            }

            return (minX, maxX, minY, maxY);
        }

        static void DisplayDeck(List<DeckCard> deck)
        {
            WriteLine("\n╔════════════════════ DECK DE CARTES ════════════════════╗");
            
            int cardsPerRow = 8;
            for (int i = 0; i < deck.Count; i++)
            {
                var card = deck[i];
                
                if (i % cardsPerRow == 0 && i > 0)
                    WriteLine();
                
                string cardDisplay = card.Bonus == Bonus.Max ? "MAX" :
                    card.Bonus == Bonus.Again ? "AGAIN" :
                    $"{card.Number:D2}";

                if (card.Bonus == Bonus.Max)
                {
                    ForegroundColor = ConsoleColor.Blue;
                }
                else if (card.Bonus == Bonus.Again)
                {
                    ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    ForegroundColor = ConsoleColor.Gray;
                }

                Write($"│ {cardDisplay,-6} ");
                ResetColor();
            }

            WriteLine("\n╚══════════════════════════════════════════════════════╝");
        }
    }
}