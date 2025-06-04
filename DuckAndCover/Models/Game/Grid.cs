using System.Runtime.Serialization;
using Models.Generators;

namespace Models.Game
{
    /// <summary>
    /// Représente la grille de jeu contenant les cartes.
    /// </summary>
    [DataContract]
    public class Grid
    {
        /// <summary>
        /// Obtient ou définit la liste des cartes dans la grille.
        /// </summary>
        [DataMember]
        public List<GameCard> GameCardsGrid { get; set; } = new(); 

        /// <summary>
        /// Initialise une nouvelle instance de la classe Grid.
        /// Génère une nouvelle grille de jeu.
        /// </summary>
        public Grid()
        {
            GridGenerator gridGenerator = new GridGenerator();
            GameCardsGrid = gridGenerator.Grid;
        }

        /// <summary>
        /// Calcule les limites de la grille à partir d'une liste de positions.
        /// </summary>
        /// <param name="positions">La liste des positions à analyser.</param>
        /// <returns>Un tuple contenant les coordonnées minimales et maximales (minX, maxX, minY, maxY).</returns>
        public static (int minX, int maxX, int minY, int maxY) GetBounds(List<Position> positions)
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

            return (minX, maxX, minY, maxY);
        }

        /// <summary>
        /// Récupère la carte à une position donnée.
        /// </summary>
        /// <param name="p">La position à vérifier.</param>
        /// <returns>La carte à la position spécifiée, ou null si aucune carte n'est présente.</returns>
        public GameCard? GetCard(Position p)
        {
            foreach (var card in GameCardsGrid)
            {
                if (card.Position.Row == p.Row && card.Position.Column == p.Column)
                {
                    return card;
                }
            }

            return null;
        }

        /// <summary>
        /// Place une nouvelle carte à une position donnée.
        /// </summary>
        /// <param name="p">La position où placer la carte.</param>
        /// <param name="newCard">La carte à placer.</param>
        public void SetCard(Position p, GameCard newCard)
        {
            if (IsInGrid(p))
            {
                RemoveCard(p);
            }

            newCard.Position = p;
            GameCardsGrid.Add(newCard);
        }

        /// <summary>
        /// Vérifie si une position est occupée dans la grille.
        /// </summary>
        /// <param name="p">La position à vérifier.</param>
        /// <returns>true si la position est occupée ; sinon, false.</returns>
        public bool IsInGrid(Position p)
        {
            return GameCardsGrid
                .Select(card => card.Position)
                .Any(pos => pos.Row == p.Row && pos.Column == p.Column);
        }

        /// <summary>
        /// Vérifie si une position est adjacente à une carte existante.
        /// </summary>
        /// <param name="p">La position à vérifier.</param>
        /// <returns>Un tuple contenant un booléen indiquant si la position est adjacente et la carte adjacente si elle existe.</returns>
        public (bool isAdjacent, GameCard? adjacentCard) IsAdjacentToCard(Position p)
        {
            foreach (var card in GameCardsGrid)
            {
                var rowDiff = Math.Abs(card.Position.Row - p.Row);
                var colDiff = Math.Abs(card.Position.Column - p.Column);

                if ((rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1))
                {
                    return (true, card);
                }
            }

            return (false, null);
        }
        
        /// <summary>
        /// Vérifie si deux positions contiennent des cartes adjacentes.
        /// </summary>
        /// <param name="p1">La première position.</param>
        /// <param name="p2">La deuxième position.</param>
        /// <returns>true si les cartes sont adjacentes ; sinon, false.</returns>
        public bool AreAdjacentCards(Position p1, Position p2)
        {
            var card1 = GetCard(p1);
            var card2 = GetCard(p2);

            if (card1 == null || card2 == null)
                return false;

            var rowDiff = Math.Abs(card1.Position.Row - card2.Position.Row);
            var colDiff = Math.Abs(card1.Position.Column - card2.Position.Column);

            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
        }

        /// <summary>
        /// Supprime la carte à une position donnée.
        /// </summary>
        /// <param name="p">La position de la carte à supprimer.</param>
        public void RemoveCard(Position p)
        {
            bool cardFound = IsInGrid(p);
            if (cardFound)
            {
                GameCardsGrid.RemoveAll(card => card.Position.Row == p.Row && card.Position.Column == p.Column);
            }
        }
    }
}