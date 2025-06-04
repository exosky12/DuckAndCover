using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Models.Game;
using Models.Exceptions;

namespace Models.Game
{
    /// <summary>
    /// Représente une IA (bot) dans le jeu.
    /// </summary>
    [DataContract]
    public class Bot : Player
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe Bot.
        /// </summary>
        /// <param name="botNumber">Le numéro du bot qui sera utilisé dans son nom.</param>
        public Bot(string botNumber)
            : base($"Bot#{botNumber}")
        {
            IsBot = true;
        }

        /// <summary>
        /// Joue automatiquement le tour du bot en suivant une stratégie prédéfinie.
        /// Le bot essaiera d'abord de faire un "cover", puis un "duck", et enfin de passer son tour avec coin.
        /// </summary>
        /// <param name="game">L'instance du jeu en cours.</param>
        public void PlayTurnAutomatically(Game game)
        {
            var player = game.CurrentPlayer;
            var deckCard = game.CurrentDeckCard;
            var grid = player.Grid;
            var rules = game.Rules;

            if (deckCard == null)
            {
                game.HandlePlayerChoice(player, "3");
                return;
            }

            var sources = grid.GameCardsGrid
                .Where(c => c.Number == deckCard.Number)
                .Select(c => c.Position)
                .ToList();

            if (!sources.Any())
            {
                game.HandlePlayerChoice(player, "3");
                return;
            }

            var priorities = new[] { "cover", "cover", "duck", "coin" }
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            foreach (var action in priorities)
            {
                if (action == "coin")
                {
                    game.HandlePlayerChoice(player, "3");
                    return;
                }

                if (action == "cover")
                {
                    for (int attempt = 0; attempt < 5; attempt++)
                    {
                        var source = sources[Random.Shared.Next(sources.Count)];
                        var targets = grid.GameCardsGrid.Select(c => c.Position).ToList();
                        if (!targets.Any()) break;

                        var destination = targets[Random.Shared.Next(targets.Count)];
                        try
                        {
                            rules.TryValidMove(source, destination, grid, "cover", deckCard);
                            game.HandlePlayerChooseCover(player, source, destination);
                            return;
                        }
                        catch (Error)
                        {
                            continue;
                        }
                    }
                }

                if (action == "duck")
                {
                    for (int attempt = 0; attempt < 5; attempt++)
                    {
                        var source = sources[Random.Shared.Next(sources.Count)];
                        var emptyPositions = GetEmptyPositions(grid);
                        if (!emptyPositions.Any()) break;

                        var destination = emptyPositions[Random.Shared.Next(emptyPositions.Count)];
                        try
                        {
                            rules.TryValidMove(source, destination, grid, "duck", deckCard);
                            game.HandlePlayerChooseDuck(player, source, destination);
                            return;
                        }
                        catch (Error)
                        {
                            continue;
                        }
                    }
                }
            }

            game.HandlePlayerChoice(player, "3");
        }

        /// <summary>
        /// Récupère la liste des positions vides dans la grille de jeu.
        /// Les positions vides sont déterminées en fonction des cartes déjà placées.
        /// </summary>
        /// <param name="grid">La grille de jeu à analyser.</param>
        /// <returns>Une liste des positions vides dans la grille.</returns>
        private static List<Position> GetEmptyPositions(Grid grid)
        {
            var occupied = grid.GameCardsGrid.Select(c => c.Position).ToHashSet();
            if (!occupied.Any())
                return new List<Position>();

            int minRow = occupied.Min(p => p.Row);
            int maxRow = occupied.Max(p => p.Row);
            int minCol = occupied.Min(p => p.Column);
            int maxCol = occupied.Max(p => p.Column);

            var empty = new List<Position>();
            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    var pos = new Position(row, col);
                    if (!occupied.Contains(pos))
                        empty.Add(pos);
                }
            }

            return empty;
        }
    }
}
