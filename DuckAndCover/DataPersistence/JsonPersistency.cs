using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using Models.Game;
using Models.Interfaces;
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
                catch (Exception ex)
                {
                    Debug.WriteLine($"[JsonPersistency] Erreur LoadData : {ex.Message}");
                    // Si le JSON est corrompu ou autre erreur, supprimer le fichier pour repartir sur une base propre
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
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[JsonPersistency] Erreur lors de la lecture du JSON existant : {ex.Message}");
                        existingPlayers = new ObservableCollection<Player>();
                        existingGames = new ObservableCollection<Game>();
                    }
                }
                else
                {
                    existingPlayers = new ObservableCollection<Player>();
                    existingGames = new ObservableCollection<Game>();
                }

                // 2) Fusionner les joueurs (ajout ou mise à jour des scores)
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

                // 3) Fusionner les parties (ajout ou mise à jour par Id)
                foreach (var newGame in allGames)
                {
                    var match = existingGames.FirstOrDefault(g => g.Id == newGame.Id);
                    if (match == null)
                    {
                        // Partie nouvelle (même Id inconnu) → on l'ajoute
                        existingGames.Add(newGame);
                    }
                    else
                    {
                        // Partie déjà présente : mise à jour des propriétés pertinentes
                        match.IsFinished = newGame.IsFinished;
                        match.LastNumber = newGame.LastNumber;
                        match.CardsSkipped = newGame.CardsSkipped;
                        match.LastGameFinishStatus = newGame.LastGameFinishStatus;
                        match.Players = newGame.Players;
                        match.Deck = newGame.Deck;
                    }
                }

                // 4) Sérialiser les données fusionnées dans le fichier JSON
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[JsonPersistency] Erreur SaveData : {ex.Message}");
                throw;
            }
        }
    }
}
