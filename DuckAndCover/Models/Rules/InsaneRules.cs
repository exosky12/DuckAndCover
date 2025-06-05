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
            "Dans cette variante, les joueurs peuvent déplacer leurs cartes de manière plus agressive.";

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

        // Note : InsaneRules utilise la validation Duck par défaut de BaseRules
        // qui correspond au comportement souhaité, donc pas besoin de surcharger ValidateDuckAdjacency
    }
}