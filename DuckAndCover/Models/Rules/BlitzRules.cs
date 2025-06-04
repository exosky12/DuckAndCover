using Models.Exceptions;
using Models.Interfaces;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    /// <summary>
    /// Implémentation des règles du jeu "Blitz".
    /// </summary>
    public class BlitzRules : IRules
    {
        /// <summary>
        /// Nom des règles.
        /// </summary>
        public string Name => "Règles Blitz";

        /// <summary>
        /// Description des règles.
        /// </summary>
        public string Description =>
            "Vivez cette partie de manière intense et rapide";

        /// <summary>
        /// Nombre de cartes dans le deck pour ces règles.
        /// </summary>
        public int NbCardsInDeck => 24;

        /// <summary>
        /// Tente de valider un mouvement de carte selon les règles Blitz.
        /// </summary>
        /// <param name="position">Position actuelle de la carte.</param>
        /// <param name="newPosition">Nouvelle position de la carte.</param>
        /// <param name="grid">Grille du jeu.</param>
        /// <param name="funcName">Nom de la fonction appelante ("duck" ou "cover").</param>
        /// <param name="currentDeckCard">Carte du deck utilisée pour valider le mouvement.</param>
        /// <exception cref="Error">Lance une exception en cas de mouvement invalide.</exception>
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

                    if (grid.IsAdjacentToCard(newPosition) == (false, null))
                        throw new Error(ErrorCodes.AdjacentCardNotFound);
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

        /// <summary>
        /// Indique si la partie est terminée selon les conditions Blitz.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de piles.</param>
        /// <param name="quit">Indique si un joueur a quitté la partie.</param>
        /// <returns>True si la partie est finie, sinon false.</returns>
        public bool IsGameOver(int cardPassed, int stackCounter, bool quit) => cardPassed == 3 || stackCounter == 5 || quit;

        /// <summary>
        /// Vérifie si une carte du jeu est la même qu'une carte du deck (par numéro).
        /// </summary>
        /// <param name="currentCard">Carte du jeu à comparer.</param>
        /// <param name="currentDeckCard">Carte du deck à comparer.</param>
        /// <returns>True si les cartes ont le même numéro, sinon false.</returns>
        public bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard)
        {
            return currentCard.Number == currentDeckCard.Number;
        }
    }
}
