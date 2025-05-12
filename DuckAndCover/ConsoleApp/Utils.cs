using static System.Console;
using Model;
namespace ConsoleApp;

public static class Utils
{
    public static void DisplayMenu()
    {
        WriteLine("\n1. Cover (recouvrir une carte)");
        WriteLine("2. Duck (déplacer une carte)");
        WriteLine("3. Call Coin (passer son tour)");
        WriteLine("4. Afficher les grilles de tous les joueurs");
        WriteLine("5. Afficher les scores");
        WriteLine("6. Quitter la partie");
    }
    
    public static void DisplayTopSeparator(string title)
    {
        WriteLine($"\n╔════════════════════ {title} ════════════════════╗");
    }
    
    public static void DisplayBottomSeparator()
    {
        WriteLine("\n╚══════════════════════════════════════════════════════╝");
    }
    
    public static void DisplayGrid(Player player)
    {
        DisplayTopSeparator("GRILLE DE JEU");
            
            // Si la grille est vide, afficher un message
            if (player.Grid.GameCardsGrid.Count == 0)
            {
                WriteLine("│                Grille actuellement vide                 │");
                DisplayBottomSeparator();
                return;
            }

            var positions = new List<Position>();
            foreach (var card in player.Grid.GameCardsGrid)
                positions.Add(card.Position);

            var (minX, maxX, minY, maxY) = player.Grid.GetBounds(positions);

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
                    var card = player.Grid.GetCard(new Position(row, col));
                    if (card != null)
                    {
                        SetSplashColor(card.Splash);
                        Write($"{card.Number} - {card.Splash} ");
                        ResetColor();
                        Write("│");
                    }
                    else
                    {
                        Write($"        │");
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
            
            DisplayBottomSeparator();
    }
    
    public static void SetSplashColor(int splash)
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
    
    
    public static void DisplayDeck(List<DeckCard> deck)
    {
        DisplayTopSeparator("DECK DE CARTES");
            
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

        DisplayBottomSeparator();
    }
    
    public static void DisplayPlayerScores(List<Player> players)
    {
        foreach (Player p in players)
        {
            WriteLine($"  {p.Name}: {p.GameScore} points");
        }
    }
    
    public static Position ParsePosition(string input)
    {
        string[] parts = input.Split(',');
        if (parts.Length != 2)
            throw new ArgumentException("Format de position invalide. Utilisez le format: ligne,colonne");
                
        if (!int.TryParse(parts[0].Trim(), out int row) || !int.TryParse(parts[1].Trim(), out int col))
            throw new ArgumentException("Les valeurs de ligne et colonne doivent être numériques");
                
        return new Position(row, col);
    }
}