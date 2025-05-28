using static System.Console;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
using Models.Exceptions;
using Models.Enums;
using System.Collections.ObjectModel;
using Models.Interfaces;
using Models.Rules;

namespace ConsoleApp;

[ExcludeFromCodeCoverage]
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

    public static void SetSplashColor(int splash)
    {
        ForegroundColor = splash switch
        {
            0 => ConsoleColor.White,
            1 => ConsoleColor.Yellow,
            2 => ConsoleColor.DarkYellow,
            3 => ConsoleColor.Red,
            4 => ConsoleColor.DarkRed,
            _ => ConsoleColor.Magenta
        };
    }

    public static string DisplayCard(DeckCard card) => card.Bonus switch
    {
        Bonus.Max => "MAX",
        Bonus.Again => "AGAIN",
        _ => $"{card.Number:D2}"
    };


    public static void DisplayPlayerScores(ObservableCollection<Player> players)
    {
        foreach (Player p in players)
        {
            WriteLine($"  {p.Name}: {p.TotalScore} points");
        }
    }

    public static Position ParsePosition(string input)
    {
        string[] parts = input.Split(',');
        if (parts.Length != 2)
            throw new Error(ErrorCodes.WrongPositionFormat);

        if (!int.TryParse(parts[0].Trim(), out int row) || !int.TryParse(parts[1].Trim(), out int col))
            throw new Error(ErrorCodes.PositionsMustBeIntegers);

        return new Position(row, col);
    }

    public static void WriteGameMaster(string message)
    {
        ConsoleColor originalColor = ForegroundColor;
        ForegroundColor = ConsoleColor.Cyan;
        WriteLine($"\n[Maître du jeu] {message}");
        ForegroundColor = originalColor;
    }

    public static void WriteError(string message)
    {
        ConsoleColor originalColor = ForegroundColor;
        ForegroundColor = ConsoleColor.Red;
        WriteLine($"\n[ERREUR] {message}");
        ForegroundColor = originalColor;
    }

    public static void ShowTitle()
    {
        string title = @"
        ______            _     ___            _ _____                     
        |  _  \          | |   / _ \          | /  __ \                    
        | | | |_   _  ___| | _/ /_\ \_ __   __| | /  \/ _____   _____ _ __ 
        | | | | | | |/ __| |/ /  _  | '_ \ / _` | |    / _ \ \ / / _ \ '__|
        | |/ /| |_| | (__|   <| | | | | | | (_| | \__/\ (_) \ V /  __/ |   
        |___/  \__,_|\___|_|\_\_| |_|_| |_|\__,_|\____/\___/ \_/ \___|_|   
                                                                           
                                                                           
        ";
        WriteGameMaster(title);
    }

    public static string PromptPlayerTurn(Player player, DeckCard card, Game game)
    {
        WriteGameMaster($"\nC'est au tour de {player.Name}");
        DisplayGrid(player);
        DisplayMenu();
        WriteGameMaster("Que souhaitez-vous faire ?");
        ProcessCardEffect(card, player, game);

        Write("\nVotre choix: ");
        return ReadLine() ?? "";
    }

    public static void DisplayGrid(Player player)
    {
        DisplayTopSeparator("GRILLE DE JEU");

        if (player.Grid.GameCardsGrid.Count == 0)
        {
            WriteLine("│                Grille actuellement vide                 │");
            DisplayBottomSeparator();
            return;
        }

        var positions = new List<Position>();
        foreach (var card in player.Grid.GameCardsGrid)
            positions.Add(card.Position);

        var (minX, maxX, minY, maxY) = Grid.GetBounds(positions);

        Write("    ");
        for (int col = minX; col <= maxX; col++)
        {
            Write($"   {col}    ");
        }

        WriteLine();

        Write("    ");
        for (int col = minX; col <= maxX; col++)
        {
            Write("────────");
        }

        WriteLine();

        for (int row = minY; row <= maxY; row++)
        {
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

            Write("    ");
            for (int col = minX; col <= maxX; col++)
            {
                Write("────────");
            }

            WriteLine();
        }

        DisplayBottomSeparator();
    }


    public static int AskNumberOfPlayers()
    {
        WriteGameMaster("Combien de joueurs ?");
        if (int.TryParse(ReadLine()!, out int nbJoueur)) return nbJoueur;
        return -1;
    }

    public static ObservableCollection<Player> InitializePlayers(int count)
    {
        var players = new ObservableCollection<Player>();
        while (players.Count < count)
        {
            WriteGameMaster($"Pseudo du joueur numéro {players.Count + 1}:");
            string name = ReadLine()!;
            while (string.IsNullOrWhiteSpace(name))
            {
                WriteGameMaster("Pseudo invalide, changez le pseudo.");
                name = ReadLine()!;
            }

            players.Add(new Player(name));
            WriteGameMaster($"Joueur {name} ajouté avec succès!");
        }

        return players;
    }

    public static void ProcessCardEffect(DeckCard card, Player player, Game game)
    {
        switch (card.Bonus)
        {
            case Bonus.Again when game.LastNumber.HasValue:
                card.Number = game.LastNumber.Value;
                WriteGameMaster($"Carte Again active ! Le numéro utilisé est {card.Number}");
                break;
            case Bonus.Max:
                int max = player.Grid.GameCardsGrid.Max(c => c.Number);
                card.Number = max;
                WriteGameMaster($"Carte MAX ! Numéro utilisé : {max} (grille de {player.Name})");
                break;
            default:
                game.LastNumber = card.Number;
                string msg = card.Number == 0 ? card.Bonus.ToString() : card.Number.ToString();
                WriteGameMaster($"Carte actuelle du deck : {msg}");
                break;
        }
    }

    public static void EndGame(ObservableCollection<Player> players, Game game)
    {
        WriteGameMaster("La partie est terminée !");
        DisplayPlayerScores(players);
    }

    public static Game CreateNewGame()
    {
        int count = AskNumberOfPlayers();
        if (count <= 0)
        {
            WriteGameMaster("Nombre de joueurs invalide. Veuillez relancer le jeu.");
            Environment.Exit(0);
        }

        var newPlayers = InitializePlayers(count);
        IRules rules = new ClassicRules();
        var newGame = new Game(rules);
        Deck deck = new Deck();
        newGame.InitializeGame(
            Guid.NewGuid().ToString("N").Substring(0, 5),
            newPlayers,
            deck,
            deck.Cards.FirstOrDefault() ?? throw new Error(ErrorCodes.DeckEmpty)
            );
        return newGame;
    }
}