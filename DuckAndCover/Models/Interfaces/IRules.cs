using Models.Game;

namespace Models.Interfaces
{
    /// <summary>
    /// Interface définissant les règles du jeu.
    /// </summary>
    public interface IRules
    {
        /// <summary>
        /// Obtient le nom des règles.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Obtient la description des règles.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Obtient le nombre de cartes dans le deck.
        /// </summary>
        int NbCardsInDeck { get; }

        /// <summary>
        /// Tente de valider un mouvement de carte selon les règles.
        /// </summary>
        /// <param name="position">Position initiale de la carte.</param>
        /// <param name="newPosition">Position cible du mouvement.</param>
        /// <param name="grid">La grille de jeu.</param>
        /// <param name="funcName">Nom de la fonction appelante (pour debug ou log).</param>
        /// <param name="currentDeckCard">La carte du deck concernée par le mouvement.</param>
        void TryValidMove(Position position, Position newPosition, Grid grid, string funcName, DeckCard currentDeckCard);

        /// <summary>
        /// Indique si la partie est terminée selon les conditions du jeu.
        /// </summary>
        /// <param name="cardPassed">Nombre de cartes passées.</param>
        /// <param name="stackCounter">Compteur de piles (stack).</param>
        /// <param name="quit">Indique si un joueur a quitté la partie.</param>
        /// <returns>True si la partie est terminée, sinon false.</returns>
        bool IsGameOver(int cardPassed, int stackCounter, bool quit);

        /// <summary>
        /// Vérifie si une carte du jeu correspond à une carte du deck.
        /// </summary>
        /// <param name="currentCard">Carte du jeu à comparer.</param>
        /// <param name="currentDeckCard">Carte du deck à comparer.</param>
        /// <returns>True si les cartes sont les mêmes, sinon false.</returns>
        bool isTheSameCard(GameCard currentCard, DeckCard currentDeckCard);
    }
}
