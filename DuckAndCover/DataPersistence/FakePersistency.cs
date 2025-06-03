using System.Diagnostics.CodeAnalysis;
using Models.Game;
using Models.Interfaces;
using System.Collections.ObjectModel;
using Models.Enums;
using Models.Exceptions;
using Models.Rules;

namespace DataPersistence
{
    /// <summary>
    /// Persistance factice en mémoire pour les tests et le développement.
    /// Simule le stockage des joueurs et parties sans accès disque.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakePersistency : IDataPersistence
    {
        /// <summary>
        /// Collection en lecture seule des joueurs chargés en mémoire.
        /// </summary>
        public ReadOnlyObservableCollection<Player> Players { get; private set; }

        private readonly ObservableCollection<Player> _players = new ObservableCollection<Player>();

        /// <summary>
        /// Collection en lecture seule des parties chargées en mémoire.
        /// </summary>
        public ReadOnlyObservableCollection<Game> Games { get; private set; }

        private readonly ObservableCollection<Game> _games = new ObservableCollection<Game>();

        /// <summary>
        /// Initialise une instance, charge les joueurs et parties factices.
        /// </summary>
        public FakePersistency()
        {
            Players = new ReadOnlyObservableCollection<Player>(_players);
            Games = new ReadOnlyObservableCollection<Game>(_games);
            LoadPlayers();
            LoadGames();
        }

        /// <summary>
        /// Charge en mémoire une liste de joueurs factices.
        /// </summary>
        public void LoadPlayers()
        {
            var loadedPlayers = GeneratePlayers();
            foreach (var player in loadedPlayers)
            {
                _players.Add(player);
            }
        }

        /// <summary>
        /// Charge en mémoire une liste de parties factices.
        /// </summary>
        public void LoadGames()
        {
            var loadedGames = GenerateGames();
            foreach (var game in loadedGames)
            {
                _games.Add(game);
            }
        }

        /// <summary>
        /// Retourne des collections observables contenant des joueurs et parties factices.
        /// </summary>
        /// <returns>Tuple (joueurs, parties) en ObservableCollection</returns>
        public (ObservableCollection<Player>, ObservableCollection<Game>) LoadData()
        {
            var players = GeneratePlayers();
            var games = GenerateGames();
            return (new ObservableCollection<Player>(players), new ObservableCollection<Game>(games));
        }

        /// <summary>
        /// Génère une liste de joueurs factices avec différentes grilles et scores.
        /// </summary>
        /// <returns>Liste de joueurs</returns>
        private ObservableCollection<Player> GeneratePlayers()
        {
            var players = new ObservableCollection<Player>();

            // Jordy avec grille modifiée (1 déplacement)
            var jordyGrid = CreateJordyGrid();
            var jordy = new Player("Jordy", 11, new List<int> { 5, 6, 3 }, false, true, jordyGrid);
            jordy.Scores.Add(30);
            players.Add(jordy);

            // Jules avec grille standard
            var jules = new Player("Jules", 12, new List<int> { 4, 7, 2 }, false, false, new Grid());
            jules.Scores.Add(30);
            jules.Scores.Add(5);
            players.Add(jules);

            // Jordy2 avec grille standard
            var jordy2 = new Player("Jordy2", 1, new List<int> { 5, 6, 3 }, false, true, new Grid());
            players.Add(jordy2);

            // Jules2 avec grille standard
            var jules2 = new Player("Jules2", 3, new List<int> { 4, 7, 2 }, false, false, new Grid());
            players.Add(jules2);

            // Jordy1 avec grille modifiée (3 déplacements)
            var jordy1Grid = CreateJordy1Grid();
            var jordy1 = new Player("Jordy1", 9, new List<int> { 5, 6, 3 }, false, true, jordy1Grid);
            players.Add(jordy1);

            // Jules1 avec grille modifiée (3 déplacements)
            var jules1Grid = CreateJules1Grid();
            var jules1 = new Player("Jules1", 9, new List<int> { 4, 7, 2 }, false, false, jules1Grid);
            jules1.Scores.Add(20);
            jules1.Scores.Add(12);
            players.Add(jules1);

            return players;
        }

        /// <summary>
        /// Génère une liste de parties factices avec divers états (en cours, terminée).
        /// </summary>
        /// <returns>Liste de parties</returns>
        private List<Game> GenerateGames()
        {
            var games = new List<Game>();
            var allPlayers = GeneratePlayers();

            // Partie en cours (7051E)
            var onGoingGame = new Game(new ClassicRules());
            Deck onGoingDeck = new Deck();
            onGoingGame.InitializeGame(
                id: "7051E",
                players: new List<Player> { allPlayers[0], allPlayers[1] }, // Jordy, Jules
                deck: onGoingDeck,
                currentPlayerIndex: 0,
                currentDeckCard: onGoingDeck.Cards.FirstOrDefault() ?? throw new Error(ErrorCodes.DeckEmpty)
            );
            games.Add(onGoingGame);

            // Partie terminée (9051e)
            var finishedGame = new Game(new ClassicRules());
            Deck finishedDeck = new Deck();

            finishedGame.InitializeGame(
                id: "9051e",
                players: new List<Player> { allPlayers[2], allPlayers[3] }, // Jordy2, Jules2
                deck: finishedDeck,
                currentPlayerIndex: 0,
                currentDeckCard: finishedDeck.Cards.FirstOrDefault() ?? throw new Error(ErrorCodes.DeckEmpty),
                cardsSkipped: 8,
                lastNumber: 1,
                isFinished: true
            );
            games.Add(finishedGame);

            // Partie intermédiaire (5051e)
            var crazyGame = new Game(new ClassicRules());
            Deck crazyDeck = new Deck();

            crazyGame.InitializeGame(
                id: "5051e",
                players: new List<Player> { allPlayers[4], allPlayers[5] }, // Jordy1, Jules1
                deck: crazyDeck,
                currentDeckCard: crazyDeck.Cards.FirstOrDefault() ?? throw new Error(ErrorCodes.DeckEmpty),
                currentPlayerIndex: 1,
                cardsSkipped: 3,
                lastNumber: 2
            );
            games.Add(crazyGame);

            return games;
        }

        /// <summary>
        /// Crée une grille modifiée avec un déplacement unique (pour Jordy).
        /// </summary>
        /// <returns>Grille modifiée</returns>
        private Grid CreateJordyGrid()
        {
            var grid = new Grid();
            // Déplacement unique : (1,1) -> (1,2)
            var cardToMove = grid.GetCard(new Position(1, 1));
            grid.RemoveCard(new Position(1, 1));
            grid.SetCard(new Position(1, 2), cardToMove);
            return grid;
        }

        /// <summary>
        /// Crée une grille modifiée avec trois déplacements (pour Jordy1).
        /// </summary>
        /// <returns>Grille modifiée</returns>
        private Grid CreateJordy1Grid()
        {
            var grid = new Grid();

            // A) (1,1) -> (2,1)
            var cardA = grid.GetCard(new Position(1, 1));
            grid.RemoveCard(new Position(1, 1));
            grid.SetCard(new Position(2, 1), cardA);

            // B) (1,2) -> (1,3)
            var cardB = grid.GetCard(new Position(1, 2));
            grid.RemoveCard(new Position(1, 2));
            grid.SetCard(new Position(1, 3), cardB);

            // C) (2,3) -> (0,0)
            var cardC = grid.GetCard(new Position(2, 3));
            grid.RemoveCard(new Position(2, 3));
            grid.SetCard(new Position(0, 0), cardC);

            return grid;
        }

        /// <summary>
        /// Crée une grille modifiée avec trois déplacements (pour Jules1).
        /// </summary>
        /// <returns>Grille modifiée</returns>
        private Grid CreateJules1Grid()
        {
            var grid = new Grid();

            // D) (2,1) -> (3,1)
            var cardD = grid.GetCard(new Position(2, 1));
            grid.RemoveCard(new Position(2, 1));
            grid.SetCard(new Position(3, 1), cardD);

            // E) (3,2) -> (3,3)
            var cardE = grid.GetCard(new Position(3, 2));
            grid.RemoveCard(new Position(3, 2));
            grid.SetCard(new Position(3, 3), cardE);

            // F) (1,4) -> (1,2)
            var cardF = grid.GetCard(new Position(1, 4));
            grid.RemoveCard(new Position(1, 4));
            grid.SetCard(new Position(1, 2), cardF);

            return grid;
        }

        /// <summary>
        /// Ne fait rien, stub pour ne pas persister les données.
        /// </summary>
        /// <param name="_players">Collection de joueurs (ignorée)</param>
        /// <param name="_games">Collection de parties (ignorée)</param>
        public void SaveData(ObservableCollection<Player> _players, ObservableCollection<Game> _games)
        {
            // Pas de persistance réelle dans cette classe car STUB
        }
    }
}
