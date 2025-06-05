using Models.Exceptions;
using Models.Interfaces;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    /// <summary>
    /// Classe de base abstraite contenant la logique commune à toutes les règles.
    /// </summary>
    public abstract class BaseRules : IRules
    {
        /// <summary>
        /// Obtient le nom des règles (à implémenter par les classes dérivées).
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Obtient la description des règles (à implémenter par les classes dérivées).
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Obtient le nombre de cartes dans le deck (à implémenter par les classes dérivées).
        /// </summary>
        public abstract int NbCardsInDeck { get; }

        /// <summary>
        /// Vérifie si une carte du jeu correspond à une carte du deck.
        /// LOGIQUE COMMUNE - identique pour toutes les règles.
        /// </summary>
        /// <param name="currentCard">Carte du jeu.</param>
        /// <param name="currentDeckCard">Carte du deck.</param>
        /// <returns>True si les cartes ont le même numéro, sinon False.</returns>
        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }

        /// <summary>
        /// Indique si la partie est terminée (à implémenter par les classes dérivées).
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de pile.</param>
        /// <param name="quit">Indique si un joueur a quitté.</param>
        /// <returns>True si la partie est terminée, sinon False.</returns>
        public abstract bool IsGameOver(int cardPassed, int stackCounter, bool quit);

        /// <summary>
        /// Tente de valider un mouvement de carte.
        /// LOGIQUE COMMUNE avec possibilité de spécialisation.
        /// </summary>
        /// <param name="position">Position actuelle de la carte.</param>
        /// <param name="newPosition">Nouvelle position souhaitée.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <param name="funcName">Nom de la fonction ("duck" ou "cover").</param>
        /// <param name="currentDeckCard">Carte du deck utilisée pour la validation.</param>
        /// <exception cref="ErrorException">Lance une erreur si le mouvement est invalide.</exception>
        public virtual void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard)
        {
            // Validations communes à toutes les règles
            ValidateCardExists(grid, position);
            ValidateCardNumber(grid, position, currentDeckCard);

            // Traitement spécifique selon l'action
            switch (funcName.ToLower())
            {
                case "duck":
                    ValidateDuckMove(position, newPosition, grid);
                    break;

                case "cover":
                    ValidateCoverMove(position, newPosition, grid);
                    break;

                default:
                    throw new ErrorException(ErrorCodes.InvalidFunctionName);
            }
        }

        /// <summary>
        /// Valide qu'une carte existe à la position donnée.
        /// </summary>
        /// <param name="grid">Grille de jeu.</param>
        /// <param name="position">Position à vérifier.</param>
        /// <exception cref="ErrorException">Lance une erreur si aucune carte n'est trouvée.</exception>
        protected virtual void ValidateCardExists(Grid grid, Position position)
        {
            GameCard? card = grid.GetCard(position);
            if (card == null)
                throw new ErrorException(ErrorCodes.CardNotFound);
        }

        /// <summary>
        /// Valide que la carte a le bon numéro par rapport à la carte du deck.
        /// </summary>
        /// <param name="grid">Grille de jeu.</param>
        /// <param name="position">Position de la carte.</param>
        /// <param name="currentDeckCard">Carte du deck de référence.</param>
        /// <exception cref="ErrorException">Lance une erreur si les numéros ne correspondent pas.</exception>
        protected virtual void ValidateCardNumber(Grid grid, Position position, DeckCard currentDeckCard)
        {
            GameCard? card = grid.GetCard(position);
            if (card != null && card.Number != currentDeckCard.Number)
                throw new ErrorException(ErrorCodes.CardNumberNotEqualToDeckCardNumber);
        }

        /// <summary>
        /// Valide un mouvement Duck.
        /// Peut être surchargée par les classes dérivées pour des règles spécifiques.
        /// </summary>
        /// <param name="currentPosition">Position actuelle de la carte.</param>
        /// <param name="newPosition">Nouvelle position souhaitée.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <exception cref="ErrorException">Lance une erreur si le mouvement est invalide.</exception>
        protected virtual void ValidateDuckMove(Position currentPosition, Position newPosition, Grid grid)
        {
            GameCard? cardToDuck = grid.GetCard(newPosition);
            if (cardToDuck != null)
                throw new ErrorException(ErrorCodes.CardAlreadyExists);

            ValidateDuckAdjacency(currentPosition, newPosition, grid);
        }

        /// <summary>
        /// Valide un mouvement Cover.
        /// Logique commune à toutes les règles.
        /// </summary>
        /// <param name="currentPosition">Position actuelle de la carte.</param>
        /// <param name="newPosition">Position de la carte à couvrir.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <exception cref="ErrorException">Lance une erreur si le mouvement est invalide.</exception>
        protected virtual void ValidateCoverMove(Position currentPosition, Position newPosition, Grid grid)
        {
            GameCard? cardToCover = grid.GetCard(newPosition);
            if (cardToCover == null)
                throw new ErrorException(ErrorCodes.CardNotFound);
            if (!grid.AreAdjacentCards(currentPosition, newPosition))
                throw new ErrorException(ErrorCodes.CardsAreNotAdjacent);
        }

        /// <summary>
        /// Valide l'adjacence pour un mouvement Duck.
        /// Méthode virtuelle permettant aux classes dérivées de personnaliser cette logique.
        /// </summary>
        /// <param name="currentPosition">Position actuelle.</param>
        /// <param name="newPosition">Nouvelle position.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <exception cref="ErrorException">Lance une erreur si l'adjacence n'est pas valide.</exception>
        protected virtual void ValidateDuckAdjacency(Position currentPosition, Position newPosition, Grid grid)
        {
            // Comportement par défaut : vérification d'adjacence simple
            if (grid.IsAdjacentToCard(newPosition) == (false, null))
                throw new ErrorException(ErrorCodes.AdjacentCardNotFound);
        }
    }
} 