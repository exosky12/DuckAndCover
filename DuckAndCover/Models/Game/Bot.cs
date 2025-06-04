using System;
using System.Linq;
using System.Collections.Generic;
using Models.Game;
using Models.Exceptions;

namespace Models.Game
{
    public class Bot : Player
    {
        private static int _botCounter = 1;
        private static readonly Random _rnd = new Random();

        public Bot()
            : base($"Bot#{_botCounter}")
        {
            _botCounter++;
            IsBot = true;
        }
        public void PlayTurnAutomatically(Game game)
        {
            var joueur   = game.CurrentPlayer;
            var deckCard = game.CurrentDeckCard;
            var grid     = joueur.Grid;
            var règles   = game.Rules;
            
            if (deckCard == null)
            {
                game.HandlePlayerChoice(joueur, "3");
                return;
            }
            
            var sources = grid.GameCardsGrid
                               .Where(c => c.Number == deckCard.Number)
                               .Select(c => c.Position)
                               .ToList();
            
            if (!sources.Any())
            {
                game.HandlePlayerChoice(joueur, "3");
                return;
            }
            var types = new[] { "cover", "duck", "coin" }
                        .OrderBy(_ => _rnd.Next())
                        .ToList();

            foreach (var t in types)
            {
                if (t == "coin")
                {
                    game.HandlePlayerChoice(joueur, "3");
                    return;
                }

                if (t == "cover")
                {
                    // Jusqu’à 5 essais de Cover aléatoire
                    for (int essai = 0; essai < 5; essai++)
                    {
                        var src    = sources[_rnd.Next(sources.Count)];
                        var cibles = grid.GameCardsGrid.Select(c => c.Position).ToList();
                        if (!cibles.Any()) break;

                        var dst = cibles[_rnd.Next(cibles.Count)];
                        try
                        {
                            règles.TryValidMove(src, dst, grid, "cover", deckCard);
                            game.HandlePlayerChooseCover(joueur, src, dst);
                            return;
                        }
                        catch (Error)
                        {
                            continue;
                        }
                    }
                }

                if (t == "duck")
                {
                    for (int essai = 0; essai < 5; essai++)
                    {
                        var src = sources[_rnd.Next(sources.Count)];
                        var empties = GetEmptyPositions(grid);
                        if (!empties.Any()) break;

                        var dst = empties[_rnd.Next(empties.Count)];
                        try
                        {
                            règles.TryValidMove(src, dst, grid, "duck", deckCard);
                            game.HandlePlayerChooseDuck(joueur, src, dst);
                            return;
                        }
                        catch (Error)
                        {
                            continue;
                        }
                    }
                }
            }
            
            game.HandlePlayerChoice(joueur, "3");
        }
        
        private static List<Position> GetEmptyPositions(Grid grid)
        {
            var occupées = grid.GameCardsGrid.Select(c => c.Position).ToHashSet();
            if (!occupées.Any())
                return new List<Position>();

            int minRow = occupées.Min(p => p.Row);
            int maxRow = occupées.Max(p => p.Row);
            int minCol = occupées.Min(p => p.Column);
            int maxCol = occupées.Max(p => p.Column);

            var empties = new List<Position>();
            for (int r = minRow; r <= maxRow; r++)
            {
                for (int c = minCol; c <= maxCol; c++)
                {
                    var pos = new Position(r, c);
                    if (!occupées.Contains(pos))
                        empties.Add(pos);
                }
            }
            return empties;
        }
    }
}
