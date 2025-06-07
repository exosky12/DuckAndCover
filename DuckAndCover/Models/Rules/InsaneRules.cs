using Models.Enums;
using Models.Exceptions;
using Models.Game;

namespace Models.Rules
{
    /// <summary>
    /// Implémentation des règles "Insanes" du jeu.
    /// Variante avec plus de cartes et des conditions de fin étendues.
    /// </summary>
    public class InsaneRules : BaseRules
    {
        /// <summary>
        /// Obtient le nom des règles.
        /// </summary>
        public override string Name => "Règles Insanes";

        /// <summary>
        /// Obtient la description des règles.
        /// </summary>
        public override string Description =>
            "Mode chaotique où tout est permis ! Recouvrez n'importe quelle carte, utilisez des cartes spéciales et jouez avec un deck plus grand.";

        /// <summary>
        /// Obtient le nombre de cartes dans le deck.
        /// </summary>
        public override int NbCardsInDeck => 128;

        /// <summary>
        /// Indique si la partie est terminée selon les règles Insanes.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de pile.</param>
        /// <param name="quit">Indique si un joueur a quitté.</param>
        /// <returns>True si la partie est terminée, sinon False.</returns>
        public override bool IsGameOver(int cardPassed, int stackCounter, bool quit) => 
            cardPassed == 16 || stackCounter == 7 || quit;

        /// <summary>
        /// Spécialisation de la validation Cover pour les règles Insanes.
        /// Permet de recouvrir des cartes non adjacentes.
        /// </summary>
        /// <param name="currentPosition">Position actuelle.</param>
        /// <param name="newPosition">Nouvelle position.</param>
        /// <param name="grid">Grille de jeu.</param>
        protected override void ValidateCoverMove(Position currentPosition, Position newPosition, Grid grid)
        {
            if (!grid.IsInGrid(newPosition))
            {
                throw new ErrorException(ErrorCodes.CardNotFound);
            }
        }

        protected override void ValidateDuckAdjacency(Position currentPosition, Position newPosition, Grid grid)
        {
            if (!grid.IsInGrid(newPosition))
            {
                throw new ErrorException(ErrorCodes.CardNotFound);
            }
        }
    }
}