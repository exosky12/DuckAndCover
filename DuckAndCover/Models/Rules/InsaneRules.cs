using Models.Exceptions;
using Models.Interfaces;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    /// <summary>
    /// Implémentation des règles "Insanes" du jeu.
    /// </summary>
    public class InsaneRules : IRules
    {
        /// <summary>
        /// Obtient le nom des règles.
        /// </summary>
        public string Name => "Règles Insanes";

        /// <summary>
        /// Obtient la description des règles.
        /// </summary>
        public string Description =>
            "Dans cette variante, les joueurs peuvent déplacer leurs cartes de manière plus agressive.";

        /// <summary>
        /// Obtient le nombre de cartes dans le deck.
        /// </summary>
        public int NbCardsInDeck => 128;

        /// <summary>
        /// Tente de valider un mouvement selon les règles Insanes.
        /// </summary>
        /// <param name="position">Position actuelle de la carte.</param>
        /// <param name="newPosition">Nouvelle position souhaitée.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <param name="funcName">Nom de la fonction (ex: "duck", "cover").</param>
        /// <param name="currentDeckCard">Carte du deck pour validation.</param>
        /// <exception cref="ErrorException">Lance une erreur si le mouvement est invalide.</exception>
        public void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard)
        {
            GameCard? card = grid.GetCard(position);
            if (card == null)
                throw new ErrorException(ErrorCodes.CardNotFound);

            if (card.Number != currentDeckCard.Number)
                throw new ErrorException(ErrorCodes.CardNumberNotEqualToDeckCardNumber);

            switch (funcName.ToLower())
            {
                case "duck":
                    GameCard? cardToDuck = grid.GetCard(newPosition);
                    if (cardToDuck != null)
                        throw new ErrorException(ErrorCodes.CardAlreadyExists);

                    if (grid.IsAdjacentToCard(newPosition) == (false, null))
                        throw new ErrorException(ErrorCodes.AdjacentCardNotFound);
                    return;

                case "cover":
                    GameCard? cardToCover = grid.GetCard(newPosition);
                    if (cardToCover == null)
                        throw new ErrorException(ErrorCodes.CardNotFound);
                    if (!grid.AreAdjacentCards(position, newPosition))
                        throw new ErrorException(ErrorCodes.CardsAreNotAdjacent);
                    return;

                default:
                    throw new ErrorException(ErrorCodes.InvalidFunctionName);
            }
        }

        /// <summary>
        /// Indique si la partie est terminée.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de pile.</param>
        /// <param name="quit">Indique si un joueur a quitté.</param>
        /// <returns>True si la partie est terminée, sinon False.</returns>
        public bool IsGameOver(int cardPassed, int stackCounter, bool quit) => cardPassed == 16 || stackCounter == 7 || quit;

        /// <summary>
        /// Vérifie si deux cartes sont identiques (même numéro).
        /// </summary>
        /// <param name="currentCard">Carte du jeu.</param>
        /// <param name="currentDeckCard">Carte du deck.</param>
        /// <returns>True si les cartes ont le même numéro, sinon False.</returns>
        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }
    }
}