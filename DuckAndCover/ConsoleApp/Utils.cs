using static System.Console;
using System.Diagnostics.CodeAnalysis;
using Models.Game;
using Models.Exceptions;
using Models.Enums;

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

    public static string DisplayCard(DeckCard card)
    {
        string cardDisplay;

        if (card.Bonus == Bonus.Max)
        {
            cardDisplay = "MAX";
        }
        else if (card.Bonus == Bonus.Again)
        {
            cardDisplay = "AGAIN";
        }
        else
        {
            cardDisplay = $"{card.Number:D2}";
        }

        return cardDisplay;
    }

    public static void DisplayPlayerScores(List<Player> players)
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
            //"Format de position invalide. Utilisez le format: ligne,colonne"
            throw new Error(1);

        if (!int.TryParse(parts[0].Trim(), out int row) || !int.TryParse(parts[1].Trim(), out int col))
            // Les valeurs de ligne et colonne doivent être numériques
            throw new Error(1);

        return new Position(row, col);
    }

    public static void WriteGameMaster(string message)
    {
        ConsoleColor originalColor = ForegroundColor;
        ForegroundColor = ConsoleColor.Cyan;
        WriteLine($"\n[Maître du jeu] {message}");
        ForegroundColor = originalColor;
    }

    public static string PromptPlayerTurn(Player player, DeckCard card, Game game)
    {
        Clear();
        WriteGameMaster($"\nC'est au tour de {player.Name}");
        DisplayGrid(player);
        DisplayMenu();
        WriteGameMaster("Que souhaitez-vous faire ?");
        ProcessCardEffect(card, player, game);

        Write("\nVotre choix: ");
        return ReadLine() ?? "";
    }


    public static int AskNumberOfPlayers()
    {
        WriteGameMaster("Combien de joueurs ?");
        if (int.TryParse(ReadLine()!, out int nbJoueur)) return nbJoueur;
        return -1;
    }

    public static List<Player> InitializePlayers(int count)
    {
        var players = new List<Player>();
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

    public static void RunGameLoop(List<Player> players, Game game, Deck deck)
    {
        bool exitGame = false;
        while (!exitGame)
        {
            
            if (deck.Cards.Count == 0)
            {
                WriteGameMaster("Le deck est vide. La partie est terminée !");
                break;
            }

            Player currentPlayer = game.CurrentPlayer;

            game.CheckGameOverCondition();

            if (game.Rules.IsGameOver(game.CardsSkipped, currentPlayer.StackCounter))
            {
                break;
            }

            if (game.AllPlayersPlayed())
                game.NextDeckCard();
            
            string choice = ReadLine()!;
            exitGame = HandlePlayerChoice(choice, currentPlayer, game, players, game.CurrentDeckCard);
            
        }
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

    public static bool AllPlayersPlayed(List<Player> players) => players.All(p => p.HasPlayed);

    public static void HandleAllPlayed(Deck deck, Game game, List<Player> players)
    {
        if(players.All(p => p.HasSkipped))
        {
            WriteGameMaster("Tous les joueurs ont passé leur tour. Carte défaussée.");
            game.CardsSkipped++;
        }
        else WriteGameMaster("Tous les joueurs ont joué. Carte retirée du deck.");
        game.Deck.Cards.RemoveAt(0);
        players.ForEach(p => p.HasSkipped = false);
        players.ForEach(p => p.HasPlayed = false);
    }

    public static void EndGame(List<Player> players, Game game)
    {
        WriteGameMaster("La partie est terminée !");
        game.Save();
        DisplayPlayerScores(players);
    }

    public static bool HandlePlayerChoice(string choice, Player player, Game game, List<Player> players, DeckCard card)
    {
        if (AllPlayersPlayed(players))
        {
            HandleAllPlayed(game.Deck, game, players);
        }
        switch (choice)
        {
            case "1":
                PerformCoverAction(player, game);
                player.HasPlayed = true;
                break;
            case "2":
                PerformDuckAction(player, game);
                player.HasPlayed = true;
                break;
            case "3":
                WriteGameMaster($"{player.Name} dit : Coin !");
                game.CallCoin(player);
                player.HasSkipped = true;
                player.HasPlayed = true;
                break;
            case "4":
                players.ForEach(p =>
                {
                    WriteGameMaster($"Grille de {p.Name}:");
                    DisplayGrid(p);
                });
                break;
            case "5":
                DisplayPlayerScores(players);
                break;
            case "6":
                WriteGameMaster("Merci d'avoir joué à Duck&Cover!");
                return true;
            default:
                WriteGameMaster("Choix invalide. Veuillez réessayer.");
                break;
        }

        return false;
    }

    private static void PerformCoverAction(Player player, Game game)
    {
        WriteGameMaster("Quelle carte souhaitez-vous utiliser pour recouvrir?");
        WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
        string fromPosition = ReadLine()!;

        WriteGameMaster("Quelle carte souhaitez-vous recouvrir?");
        WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,2");
        string toPosition = ReadLine()!;

        try
        {
            game.DoCover(player, ParsePosition(fromPosition), ParsePosition(toPosition));
            // var fromPos = ParsePosition(fromPosition);
            // var toPos = ParsePosition(toPosition);
            //
            // GameCard? fromCard = player.Grid.GetCard(fromPos);
            // GameCard? toCard = player.Grid.GetCard(toPos);
            //
            // if (fromCard == null || toCard == null)
            // {
            //     WriteGameMaster("Une des positions ne contient pas de carte!");
            //     return;
            // }
            //
            // if (!game.Rules.isTheSameCard(fromCard, currentDeckCard))
            // {
            //     WriteGameMaster("Impossible de jouer cette carte car ce n'est pas la carte actuelle");
            //     return;
            // }
            //
            // WriteGameMaster($"Tentative de recouvrir la carte {toCard.Number} (splash {toCard.Splash}) " +
            //                 $"avec la carte {fromCard.Number} (splash {fromCard.Splash})");
            //
            // bool success = player.Cover(fromCard, toCard, player.Grid, game);
            //
            // if (success)
            // {
            //     WriteGameMaster("Recouvrement réussi!");
            // }
            // else WriteGameMaster("Recouvrement impossible avec ces cartes.");
        }
        catch (Error e)
        {
            ErrorHandler errorHandler = new ErrorHandler(e.ErrorCode);
            string errorMessage = errorHandler.Handle();
            WriteGameMaster($"Erreur: {errorMessage}");
        }
    }

    private static void PerformDuckAction(Player player, Game game)
    {
        WriteGameMaster("Quelle carte souhaitez-vous déplacer?");
        WriteGameMaster("Entrez la position (ligne,colonne) - exemple: 1,1");
        string fromPosition = ReadLine()!;

        WriteGameMaster("Où souhaitez-vous la déplacer?");
        WriteGameMaster("Entrez la nouvelle position (ligne,colonne) - exemple: 2,3");
        string toPosition = ReadLine()!;
        try
        {
            game.DoDuck(player, ParsePosition(fromPosition), ParsePosition(toPosition));
            // var fromPos = ParsePosition(fromPosition);
            // var toPos = ParsePosition(toPosition);
            //
            // GameCard? cardToMove = player.Grid.GetCard(fromPos);
            //
            // if (cardToMove == null)
            // {
            //     WriteGameMaster("Aucune carte à déplacer à cette position.");
            //     return;
            // }
            //
            // bool success = player.Duck(cardToMove, toPos, player.Grid, game);
            //
            // if (success)
            // {
            //     WriteGameMaster("Déplacement effectué avec succès.");
            // }
            // else
            // {
            //     WriteGameMaster("Déplacement impossible.");
            // }
        }
        catch (Error e)
        {
            ErrorHandler errorHandler = new ErrorHandler(e.ErrorCode);
            string errorMessage = errorHandler.Handle();
            WriteGameMaster($"Erreur: {errorMessage}");
        }
    }
}
