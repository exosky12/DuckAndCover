using Models.Exceptions;
using Models.Enums;
using Models.Game;

namespace Models.Rules
{
    /// <summary>
    /// Implémentation des règles classiques du jeu.
    /// </summary>
    public class ClassicRules : BaseRules
    {
        /// <summary>
        /// Obtient le nom des règles.
        /// </summary>
        public override string Name => "Règles Classiques";

        /// <summary>
        /// Obtient la description des règles.
        /// </summary>
        public override string Description =>
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris";

        /// <summary>
        /// Obtient le nombre de cartes dans le deck.
        /// </summary>
        public override int NbCardsInDeck => 24;
        
        /// <summary>
        /// Indique si la partie est terminée selon les règles classiques.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de piles.</param>
        /// <param name="quit">Indique si un joueur a quitté la partie.</param>
        /// <returns>True si la partie est terminée, sinon False.</returns>
        public override bool IsGameOver(int cardPassed, int stackCounter, bool quit) => 
            cardPassed == 8 || stackCounter == 1 || quit;

        /// <summary>
        /// Spécialisation de la validation Duck pour les règles classiques.
        /// </summary>
        /// <param name="currentPosition">Position actuelle.</param>
        /// <param name="newPosition">Nouvelle position.</param>
        /// <param name="grid">Grille de jeu.</param>
        /// <exception cref="ErrorException">Lance une erreur si l'adjacence n'est pas valide.</exception>
        protected override void ValidateDuckAdjacency(Position currentPosition, Position newPosition, Grid grid)
        {
            (bool isAdjacent, GameCard? adjacentCard) = grid.IsAdjacentToCard(newPosition);
            
            if (!isAdjacent || Equals(adjacentCard?.Position, currentPosition))
                throw new ErrorException(ErrorCodes.CardsAreNotAdjacent);
        }
    }
}