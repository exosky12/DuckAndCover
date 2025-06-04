using System.Runtime.Serialization;
using Models.Exceptions;
using Models.Interfaces;

namespace Models.Game
{
    /// <summary>
    /// Représente une IA (bot) dans le jeu.
    /// </summary>
    [DataContract]
    public class Bot : Player
    {
        // <summary>        
        /// Liste des actions que le bot peut faire.
        /// (2 fois cover pour augmenter les chances d'avoir un cover)
        /// 
        private static readonly List<string> Priorities = new List<string> { "cover", "cover", "duck", "coin" };
        
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
        /// Joue automatiquement le tour du joueur courant dans la partie.
        /// </summary>
        /// <param name="game">L'instance du jeu en cours.</param>
        public static void PlayTurnAutomatically(Game game)
        {
            var player = game.CurrentPlayer;
            var deckCard = game.CurrentDeckCard;
            var grid = player.Grid;
            var rules = game.Rules;

            if (deckCard == null || !HasMatchingCards(grid, deckCard))
            {
                game.HandlePlayerChoice(player, "3");
                return;
            }

            var priorities = GetRandomizedPriorities();

            if (priorities.Any(action => TryExecuteAction(action, game, player, grid, rules, deckCard)))
                return;

            game.HandlePlayerChoice(player, "3");
        }

        /// <summary>
        /// Vérifie si la grille contient des cartes correspondant au numéro de la carte du deck.
        /// </summary>
        /// <param name="grid">La grille de jeu du joueur.</param>
        /// <param name="deckCard">La carte actuelle du deck.</param>
        /// <returns>True si au moins une carte correspondante est trouvée, sinon false.</returns>
        private static bool HasMatchingCards(Grid grid, DeckCard deckCard)
        {
            return grid.GameCardsGrid.Any(c => c.Number == deckCard.Number);
        }

        /// <summary>
        /// Obtient une liste aléatoirement ordonnée des actions prioritaires possibles.
        /// </summary>
        /// <returns>Liste des actions à tenter dans un ordre aléatoire.</returns>
        private static List<string> GetRandomizedPriorities()
        {
            return Priorities.OrderBy(_ => Random.Shared.Next()).ToList();
        }

        /// <summary>
        /// Tente d'exécuter une action donnée pour le joueur courant.
        /// </summary>
        /// <param name="action">Le nom de l'action à tenter ("cover", "duck", "coin").</param>
        /// <param name="game">L'instance du jeu en cours.</param>
        /// <param name="player">Le joueur courant.</param>
        /// <param name="grid">La grille de jeu du joueur.</param>
        /// <param name="rules">Les règles du jeu en cours.</param>
        /// <param name="deckCard">La carte actuelle du deck.</param>
        /// <returns>True si l'action a été exécutée avec succès, sinon false.</returns>
        private static bool TryExecuteAction(string action, Game game, Player player, Grid grid, IRules rules,
            DeckCard deckCard)
        {
            if (action == "coin")
            {
                game.HandlePlayerChoice(player, "3");
                return true;
            }

            if (action == "cover")
                return TryCover(game, player, grid, rules, deckCard);

            if (action == "duck")
                return TryDuck(game, player, grid, rules, deckCard);

            return false;
        }

        /// <summary>
        /// Tente d'exécuter l'action "cover" sur la grille.
        /// </summary>
        private static bool TryCover(Game game, Player player, Grid grid, IRules rules, DeckCard deckCard)
        {
            var sources = grid.GameCardsGrid.Where(c => c.Number == deckCard.Number).Select(c => c.Position).ToList();
            var targets = grid.GameCardsGrid.Select(c => c.Position).ToList();
            if (targets.Count == 0) return false;

            for (int i = 0; i < 5; i++)
            {
                var source = GetRandomItem(sources);
                var destination = GetRandomItem(targets);

                if (TryMove(game, player, grid, rules, deckCard, source, destination, "cover"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tente d'exécuter l'action "duck" sur la grille.
        /// </summary>
        private static bool TryDuck(Game game, Player player, Grid grid, IRules rules, DeckCard deckCard)
        {
            var sources = grid.GameCardsGrid.Where(c => c.Number == deckCard.Number).Select(c => c.Position).ToList();
            var emptyPositions = GetEmptyPositions(grid);
            if (emptyPositions.Count == 0) return false;

            for (int i = 0; i < 5; i++)
            {
                var source = GetRandomItem(sources);
                var destination = GetRandomItem(emptyPositions);

                if (TryMove(game, player, grid, rules, deckCard, source, destination, "duck"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tente d'exécuter un déplacement valide selon les règles du jeu.
        /// </summary>
        /// <param name="game">L'instance du jeu en cours.</param>
        /// <param name="player">Le joueur courant.</param>
        /// <param name="grid">La grille de jeu du joueur.</param>
        /// <param name="rules">Les règles du jeu.</param>
        /// <param name="deckCard">La carte du deck utilisée.</param>
        /// <param name="source">La position source du déplacement.</param>
        /// <param name="destination">La position destination du déplacement.</param>
        /// <param name="action">Le type d'action ("cover" ou "duck").</param>
        /// <returns>True si le déplacement est valide et exécuté, sinon false.</returns>
        private static bool TryMove(Game game, Player player, Grid grid, IRules rules, DeckCard deckCard,
            Position source, Position destination, string action)
        {
            try
            {
                rules.TryValidMove(source, destination, grid, action, deckCard);

                if (action == "cover")
                    game.HandlePlayerChooseCover(player, source, destination);
                else if (action == "duck")
                    game.HandlePlayerChooseDuck(player, source, destination);

                return true;
            }
            catch (ErrorException)
            {
                return false;
            }
        }

        /// <summary>
        /// Retourne un élément aléatoire dans une liste.
        /// </summary>
        private static T GetRandomItem<T>(List<T> list)
        {
            return list[Random.Shared.Next(list.Count)];
        }

        /// <summary>
        /// Récupère la liste des positions vides dans la grille de jeu.
        /// Les positions vides sont déterminées en fonction des cartes déjà placées.
        /// </summary>
        /// <param name="grid">La grille de jeu à analyser.</param>
        /// <returns>Une liste des positions vides dans la grille.</returns>
        public static List<Position> GetEmptyPositions(Grid grid)
        {
            var occupied = grid.GameCardsGrid.Select(c => c.Position).ToHashSet();
            if (occupied.Count == 0)
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