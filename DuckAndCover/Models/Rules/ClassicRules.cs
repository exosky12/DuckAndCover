using Models.Exceptions;
using Models.Interfaces;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    /// <summary>
    /// Implémentation des règles classiques du jeu.
    /// </summary>
    public class ClassicRules : IRules
    {
        /// <summary>
        /// Obtient le nom des règles.
        /// </summary>
        public string Name => "Règles Classiques";

        /// <summary>
        /// Obtient la description des règles.
        /// </summary>
        public string Description =>
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris";

        /// <summary>
        /// Obtient le nombre de cartes dans le deck.
        /// </summary>
        public int NbCardsInDeck => 24;
        
        /// <summary>
        /// Indique si la partie est terminée.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de piles.</param>
        /// <param name="quit">Indique si un joueur a quitté la partie.</param>
        /// <returns>True si la partie est terminée, sinon False.</returns>
        public bool IsGameOver(int cardPassed, int stackCounter, bool quit) => cardPassed == 8 || stackCounter == 1 || quit;

        /// <summary>
        /// Vérifie si une carte du jeu correspond à une carte du deck.
        /// </summary>
        /// <param name="currentCard">Carte du jeu.</param>
        /// <param name="currentDeckCard">Carte du deck.</param>
        /// <returns>True si les cartes ont le même numéro, sinon False.</returns>
        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }

        /// <summary>
        /// Tente de valider un mouvement de carte selon les règles classiques.
        /// </summary>
        /// <param name="position">Position actuelle de la carte.</param>
        /// <param name="newPosition">Nouvelle position souhaitée.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <param name="funcName">Nom de la fonction ("duck" ou "cover").</param>
        /// <param name="currentDeckCard">Carte du deck utilisée pour la validation.</param>
        /// <exception cref="Error">Lance une erreur si le mouvement est invalide.</exception>
        public void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard)
        {
            GameCard? card = grid.GetCard(position);
            if (card == null)
                throw new Error(ErrorCodes.CardNotFound);

            if (card.Number != currentDeckCard.Number)
                throw new Error(ErrorCodes.CardNumberNotEqualToDeckCardNumber);

            switch (funcName.ToLower())
            {
                case "duck":
                    GameCard? cardToDuck = grid.GetCard(newPosition);
                    if (cardToDuck != null)
                        throw new Error(ErrorCodes.CardAlreadyExists);
                    
                    (bool isAdjacent, GameCard? adjacentCard) = grid.IsAdjacentToCard(newPosition);
                    
                    if (!isAdjacent || Equals(adjacentCard?.Position, position))
                        throw new Error(ErrorCodes.CardsAreNotAdjacent);

                    return;

                case "cover":
                    GameCard? cardToCover = grid.GetCard(newPosition);
                    if (cardToCover == null)
                        throw new Error(ErrorCodes.CardNotFound);
                    if (!grid.AreAdjacentCards(position, newPosition))
                        throw new Error(ErrorCodes.CardsAreNotAdjacent);
                    return;

                default:
                    throw new Error(ErrorCodes.InvalidFunctionName);
            }
        }
    }
}