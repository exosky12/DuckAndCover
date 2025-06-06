using System.Runtime.Serialization;

namespace Models.Game
{
    /// <summary>
    /// Représente un joueur dans le jeu avec ses attributs et son état.
    /// </summary>
    [KnownType(typeof(Bot))]
    [DataContract]
    public class Player
    {
        /// <summary>
        /// Obtient le nom du joueur.
        /// </summary>
        [DataMember]
        public string Name { get; init; }
        
        /// <summary>
        /// Obtient ou définit si le joueur a passé son tour.
        /// </summary>
        [DataMember]
        public bool HasSkipped { get; set; }
        
        /// <summary>
        /// Obtient ou définit si le joueur a joué son tour.
        /// </summary>
        [DataMember]
        public bool HasPlayed { get; set; }
        
        [DataMember]
        public bool IsBot { get; set; } 
        
        /// <summary>
        /// Obtient ou définit la liste des scores du joueur.
        /// </summary>
        [DataMember]
        public List<int> Scores { get; set; } = new List<int>();
        
        /// <summary>
        /// Obtient le score total du joueur.
        /// </summary>
        public int TotalScore => Scores.Sum();

        /// <summary>
        /// Obtient ou définit le compteur de cartes restantes du joueur.
        /// </summary>
        [DataMember]
        public int StackCounter { get; set; }

        /// <summary>
        /// Obtient la grille de jeu du joueur.
        /// </summary>
        [DataMember]
        public Grid Grid { get; private set; } = new Grid();

        /// <summary>
        /// Initialise une nouvelle instance de la classe Player.
        /// </summary>
        /// <param name="name">Le nom du joueur.</param>
        public Player(string name)
        {
            this.StackCounter = 12;
            this.Scores = new List<int>();
            this.Name = name;
            this.IsBot = false;
            this.HasSkipped = false;
            this.HasPlayed = false;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Player avec des paramètres spécifiques.
        /// </summary>
        /// <param name="name">Le nom du joueur.</param>
        /// <param name="stack">Le nombre de cartes restantes.</param>
        /// <param name="scores">La liste des scores.</param>
        /// <param name="skipped">Si le joueur a passé son tour.</param>
        /// <param name="played">Si le joueur a joué son tour.</param>
        /// <param name="grid">La grille de jeu du joueur.</param>
        public Player(string name, int stack, List<int> scores, bool skipped, bool played, Grid grid)
        {
            this.Name = name;
            this.StackCounter = stack;
            this.Scores = new List<int>();
            this.HasSkipped = skipped;    
            this.HasPlayed = played;
            this.Grid = grid;
        }
        
        /// <summary>
        /// Vérifie si le joueur possède une carte avec un numéro spécifique.
        /// </summary>
        /// <param name="number">Le numéro de la carte à rechercher.</param>
        /// <returns>true si le joueur possède la carte ; sinon, false.</returns>
        public bool HasCardWithNumber(int number)
        {
            foreach (var card in Grid.GameCardsGrid)
            {
                if (card.Number == number)
                    return true;
            }

            return false;
        }
    }
}