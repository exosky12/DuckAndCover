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
            WriteLine("Bienvenue sur Duck&Cover\n");
            WriteLine("Combien de joueurs ?");
            string input = ReadLine();
            int nbJoueur;

            if (int.TryParse(input, out nbJoueur))
            {
                List<Player> players = new List<Player>();
                while (players.Count < nbJoueur)
                {
                    WriteLine($"Pseudo du joueur numéro {players.Count + 1}:");
                    string playerName = ReadLine();
                    while (string.IsNullOrEmpty(playerName))
                    {
                        WriteLine("Pseudo invalide, changez le pseudo.");
                        playerName = ReadLine();
                    }

                    Player player = new Player(playerName);
                    players.Add(player);
                }

                Player testPlayer = players[0];

                DisplayGrid(testPlayer.Grid);

                DeckGenerator deckGenerator = new DeckGenerator();
                deckGenerator.Generate();

                DisplayDeck(deckGenerator.Deck);

                foreach (Player p in players)
                {
                    WriteLine($"\nJoueur: {p.Name} - Score: {p.GameScore}");
                }

                GameCard card1 = testPlayer.Grid.GameCardsGrid[0];
                GameCard card2 = testPlayer.Grid.GameCardsGrid[1];
                Game game = new Game(players);

                WriteLine("\nTest de la fonction Cover:");

                Position from = new Position(card1.Position.Row, card1.Position.Column);
                Position to = new Position(card2.Position.Row, card2.Position.Column);

                WriteLine($"Carte de numéro: {card1.Number} et de splash {card1.Splash}, " +
                          $"de la ligne {from.Row + 1} colonne {from.Column + 1} " +
                          $"va recouvrir la carte de numéro: {card2.Number} et de splash {card2.Splash}, " +
                          $"de la ligne {to.Row + 1} colonne {to.Column + 1}.");

                testPlayer.Cover(card1, card2, testPlayer.Grid, game);

                WriteLine("\nGrille après le mouvement de couverture :");
                DisplayGrid(testPlayer.Grid);

                WriteLine("\nTest de la fonction Duck:");

                GameCard duckedCard = testPlayer.Grid.GetCard(card1.Position);
                Position oldDuckPos = new Position(duckedCard.Position.Row, duckedCard.Position.Column);
                Position newDuckPos = new Position(3, 3);

                WriteLine($"Tentative de déplacement de la carte {duckedCard.Number} (splash {duckedCard.Splash}) " +
                          $"de la position ({oldDuckPos.Row + 1}, {oldDuckPos.Column + 1}) " +
                          $"vers la position ({newDuckPos.Row + 1}, {newDuckPos.Column + 1}).");

                testPlayer.Duck(duckedCard, newDuckPos, testPlayer.Grid, game);

                GameCard movedCard = testPlayer.Grid.GetCard(newDuckPos);

                if (movedCard == duckedCard)
                {
                    WriteLine("Déplacement réussi !");
                }
                else
                {
                    WriteLine("Déplacement invalide, passage au joueur suivant.");
                }

                WriteLine("\nGrille après le mouvement Duck:");
                DisplayGrid(testPlayer.Grid);

                WriteLine("\nTest de la fonction CallCoin:");
                testPlayer.CallCoin();
            }
        }

        static void DisplayGrid(Grid grid)
        {
            WriteLine("\n================== Grille de jeu ==================");

            var positions = new List<Position>();
            foreach (var card in grid.GameCardsGrid)
                positions.Add(card.Position);

            var (minX, maxX, minY, maxY) = GetBounds(positions);

            Write("    ");
            for (int col = minX; col <= maxX; col++)
            {
                Write($"  {col + 1,-2}   ");
            }

            WriteLine();

            Write("    ");
            for (int col = minX; col <= maxX; col++)
            {
                Write("-------");
            }

            WriteLine();

            for (int row = minY; row <= maxY; row++)
            {
                Write($" {row + 1,-2} |");

                for (int col = minX; col <= maxX; col++)
                {
                    var card = grid.GetCard(new Position(row, col));
                    if (card != null)
                    {
                        SetSplashColor(card.Splash);
                        Write($" {card.Number:D2}-{card.Splash,-4}");
                        ResetColor();
                        Write("|");
                    }
                    else
                    {
                        Write($"        |");
                    }
                }

                WriteLine();

                Write("    ");
                for (int col = minX; col <= maxX; col++)
                {
                    Write("-------");
                }

                WriteLine();
            }
        }

        static void SetSplashColor(int splash)
        {
            if (splash <= 0)
            {
                ForegroundColor = ConsoleColor.White;
            }
            else if (splash < 20)
            {
                ForegroundColor = ConsoleColor.Yellow;
            }
            else if (splash < 40)
            {
                ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (splash < 60)
            {
                ForegroundColor = ConsoleColor.Red;
            }
            else if (splash < 80)
            {
                ForegroundColor = ConsoleColor.DarkRed;
            }
            else
            {
                ForegroundColor = ConsoleColor.Magenta;
                BackgroundColor = ConsoleColor.DarkRed;
            }
        }

        static (int minX, int maxX, int minY, int maxY) GetBounds(List<Position> positions)
        {
            // Même implémentation que dans le code original...
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

            return (minX, maxX, minY, maxY);
        }

        static void DisplayDeck(List<DeckCard> deck)
        {
            WriteLine("\n==================== Deck de cartes ====================");
            foreach (var card in deck)
            {
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

                Write($"| {cardDisplay,-6} ");
                ResetColor();
            }

            WriteLine("\n----------------------------------------------------------");
        }
    }
}