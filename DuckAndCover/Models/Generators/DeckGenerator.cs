using System.Security.Cryptography;
using Models.Interfaces;
using Models.Game;
using Models.Enums;

namespace Models.Generators
{
    /// <summary>
    /// Générateur de deck de cartes pour le jeu.
    /// </summary>
    public class DeckGenerator : IGenerator<DeckCard>
    {
        /// <summary>
        /// Obtient le deck généré.
        /// </summary>
        public List<DeckCard> Deck { get; private set; } = new List<DeckCard>();

        /// <summary>
        /// Obtient la liste de toutes les cartes possibles.
        /// </summary>
        public List<DeckCard> AllPossibleCards { get; private set; } = InitializeDeck();

        /// <summary>
        /// Initialise le deck avec toutes les cartes possibles.
        /// </summary>
        /// <returns>Une liste contenant toutes les cartes possibles.</returns>
        private static List<DeckCard> InitializeDeck()
        {
            var cards = new List<DeckCard>();

            for (int i = 0; i < 2; i++)
            {
                for (int number = 1; number <= 12; number++)
                {
                    cards.Add(new DeckCard(number));
                }
            }

            cards.Add(new DeckCard(Bonus.Max, 0));
            cards.Add(new DeckCard(Bonus.Again, 0));

            return cards;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe DeckGenerator.
        /// </summary>
        public DeckGenerator()
        {
            Generate();
        }

        /// <summary>
        /// Génère un nouveau deck de cartes mélangé aléatoirement.
        /// </summary>
        /// <returns>La liste des cartes du deck généré.</returns>
        public List<DeckCard> Generate()
        {
            Deck.Clear();
            List<DeckCard> copy = new List<DeckCard>(AllPossibleCards);

            while (copy.Count > 0)
            {
                int index = GetSecureRandomIndex(copy.Count);
                Deck.Add(copy[index]);
                copy.RemoveAt(index);
            }

            return Deck;
        }

        /// <summary>
        /// Génère un index aléatoire sécurisé.
        /// </summary>
        /// <param name="max">La valeur maximale (exclusive) pour l'index.</param>
        /// <returns>Un index aléatoire entre 0 et max-1.</returns>
        private static int GetSecureRandomIndex(int max)
        {
            return RandomNumberGenerator.GetInt32(max);
        }
    }
}