namespace Models.Rules
{
    /// <summary>
    /// Implémentation des règles du jeu "Blitz".
    /// Règles rapides et intenses avec conditions de fin modifiées.
    /// </summary>
    public class BlitzRules : BaseRules
    {
        /// <summary>
        /// Nom des règles.
        /// </summary>
        public override string Name => "Règles Blitz";

        /// <summary>
        /// Description des règles.
        /// </summary>
        public override string Description =>
            "Vivez cette partie de manière intense et rapide";

        /// <summary>
        /// Nombre de cartes dans le deck pour ces règles.
        /// </summary>
        public override int NbCardsInDeck => 24;

        /// <summary>
        /// Indique si la partie est terminée selon les conditions Blitz.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de piles.</param>
        /// <param name="quit">Indique si un joueur a quitté la partie.</param>
        /// <returns>True si la partie est finie, sinon false.</returns>
        public override bool IsGameOver(int cardPassed, int stackCounter, bool quit) => 
            cardPassed == 3 || stackCounter == 5 || quit;

        // Note : BlitzRules utilise la validation Duck par défaut de BaseRules
        // qui est identique à InsaneRules, donc pas besoin de surcharger ValidateDuckAdjacency
    }
}