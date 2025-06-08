using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using Models.Game;
using Models.Interfaces;
using Models.Exceptions;
using Models.Enums;
using DTOs;

namespace DataPersistence
{
    /// <summary>
    /// Classe responsable de la persistance des données au format JSON.
    /// </summary>
    public class JsonPersistency : IDataPersistence
    {
        /// <summary>
        /// Nom du fichier JSON où les données sont sauvegardées.
        /// </summary>
        public string FileName { get; set; } = "duckAndCover_data.json";

        /// <summary>
        /// Chemin complet vers le dossier contenant le fichier JSON.
        /// </summary>
        public string FilePath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DuckAndCover"
            );

        /// <summary>
        /// Charge les données depuis le fichier JSON.
        /// </summary>
        /// <returns>
        /// Un tuple contenant les collections observables des joueurs et des parties.
        /// </returns>
        public (ObservableCollection<Player>, ObservableCollection<Game>) LoadData()
        {
            string fullPath = Path.Combine(FilePath, FileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    using var stream = File.OpenRead(fullPath);
                    
                    var serializer = new DataContractJsonSerializer(typeof(DataToPersistDto));
                    var data = (DataToPersistDto)serializer.ReadObject(stream)!;
                    Debug.WriteLine(data);
                    return (
                        data.Players ?? new ObservableCollection<Player>(),
                        data.Games ?? new ObservableCollection<Game>()
                    );
                }
                catch (ErrorException ex)
                {
                    var handler = new ErrorHandler(ex);
                    Debug.WriteLine($"[JsonPersistency] ErrorException LoadData : {handler.Handle()}");
                    try
                    {
                        File.Delete(fullPath);
                    }
                    catch { }

                    return (
                        new ObservableCollection<Player>(),
                        new ObservableCollection<Game>()
                    );
                }
            }
            else
            {
                return (
                    new ObservableCollection<Player>(),
                    new ObservableCollection<Game>()
                );
            }
        }

        /// <summary>
        /// Sauvegarde les données des joueurs et des parties dans le fichier JSON.
        /// </summary>
        /// <param name="allPlayers">Collection des joueurs à sauvegarder.</param>
        /// <param name="allGames">Collection des parties à sauvegarder.</param>
        /// <exception cref="Exception">Relance toute exception survenue lors de la sauvegarde.</exception>
        public void SaveData(ObservableCollection<Player> allPlayers, ObservableCollection<Game> allGames)
        {
            try
            {
                Directory.CreateDirectory(FilePath);
                string fullPath = Path.Combine(FilePath, FileName);
                
                ObservableCollection<Player>existingPlayers;
                ObservableCollection<Game> existingGames;

                if (File.Exists(fullPath))
                {
                    try
                    {
                        using var readStream = File.OpenRead(fullPath);
                        var serializer = new DataContractJsonSerializer(typeof(DataToPersistDto));
                        var oldData = (DataToPersistDto)serializer.ReadObject(readStream)!;

                        existingPlayers = oldData.Players ?? new ObservableCollection<Player>();
                        existingGames = oldData.Games ?? new ObservableCollection<Game>();
                    }
                    catch (ErrorException ex)
                    {
                        var handler = new ErrorHandler(ex);
                        Debug.WriteLine($"[JsonPersistency] ErrorException lors de la lecture du JSON existant : {handler.Handle()}");
                        existingPlayers = new ObservableCollection<Player>();
                        existingGames = new ObservableCollection<Game>();
                    }
                }
                else
                {
                    existingPlayers = new ObservableCollection<Player>();
                    existingGames = new ObservableCollection<Game>();
                }

                foreach (var newPlayer in allPlayers)
                {
                    var match = existingPlayers.FirstOrDefault(p => p.Name == newPlayer.Name);
                    if (match == null)
                    {
                        existingPlayers.Add(newPlayer);
                    }
                    else
                    {
                        foreach (var score in newPlayer.Scores)
                        {
                            if (!match.Scores.Contains(score))
                                match.Scores.Add(score);
                        }
                    }
                }

                foreach (var newGame in allGames)
                {
                    var match = existingGames.FirstOrDefault(g => g.Id == newGame.Id);
                    if (match == null)
                    {
                        existingGames.Add(newGame);
                    }
                    else
                    {
                        match.IsFinished = newGame.IsFinished;
                        match.LastNumber = newGame.LastNumber;
                        match.CardsSkipped = newGame.CardsSkipped;
                        match.LastGameFinishStatus = newGame.LastGameFinishStatus;
                        match.Players = newGame.Players;
                        match.Deck = newGame.Deck;
                    }
                }

                using var writeStream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
                var persistSerializer = new DataContractJsonSerializer(typeof(DataToPersistDto));
                var mergedData = new DataToPersistDto
                {
                    Players = existingPlayers,
                    Games = existingGames
                };
                persistSerializer.WriteObject(writeStream, mergedData);
                writeStream.Flush();

                Debug.WriteLine($"[JsonPersistency] Données fusionnées et réécrites dans {fullPath}");
            }
            catch (ErrorException ex)
            {
                var handler = new ErrorHandler(ex);
                Debug.WriteLine($"[JsonPersistency] ErrorException SaveData : {handler.Handle()}");
                throw;
            }
        }



        public Game? LoadLastUnfinishedGame(IRules rules)
        {
            var (_, allGames) = LoadData();
            var savedGame = allGames
                .Where(g => !g.IsFinished)
                .OrderByDescending(g => g.SavedAt) 
                .FirstOrDefault();

            if (savedGame == null)
                return null;

            Debug.WriteLine($"Players count: {savedGame.Players?.Count ?? 0}");
            Debug.WriteLine($"Current player index: {savedGame._currentPlayerIndex}");
            if (savedGame.Players?.Count > 0)
            {
                Debug.WriteLine($"First player name: {savedGame.Players[0].Name}");
            }

            var restoredGame = new Game(rules);
            restoredGame.InitializeGame(
                id: savedGame.Id,
                players: savedGame.Players ?? new List<Player>(),
                deck: savedGame.Deck ?? new Deck(),
                currentDeckCard: savedGame.CurrentDeckCard,
                currentPlayerIndex: savedGame._currentPlayerIndex,
                cardsSkipped: savedGame.CardsSkipped,
                isFinished: savedGame.IsFinished,
                lastNumber: savedGame.LastNumber
            );
            
            Debug.WriteLine($"Restored current player index: {restoredGame._currentPlayerIndex}");
            Debug.WriteLine($"Current player is null: {restoredGame.CurrentPlayer == null}");
            
            
            restoredGame.LastGameFinishStatus = savedGame.LastGameFinishStatus;
            restoredGame.SavedAt = savedGame.SavedAt;
            restoredGame.Quit = savedGame.Quit;
            restoredGame.Games = new ObservableCollection<Game>(allGames);
            var (allPlayers, _) = LoadData();
            restoredGame.AllPlayers = new ObservableCollection<Player>(allPlayers);
            Debug.WriteLine(restoredGame);
            return restoredGame;
        }
    }
}
