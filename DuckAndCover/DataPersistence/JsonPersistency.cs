using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using Models.Game;
using Models.Interfaces;

namespace DataPersistence
{
    public class JsonPersistency : IDataPersistence
    {
        public string FileName { get; set; } = "duckAndCover_data.json";

        public string FilePath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DuckAndCover"
            );

        public (ObservableCollection<Player>, ObservableCollection<Game>) LoadData()
        {
            string fullPath = Path.Combine(FilePath, FileName);
            if (!Directory.Exists(FilePath) || !File.Exists(fullPath))
            {
                return (
                    new ObservableCollection<Player>(),
                    new ObservableCollection<Game>()
                );
            }

            try
            {
                using var stream = File.OpenRead(fullPath);
                var serializer = new DataContractJsonSerializer(typeof(DataToPersist));
                var data = (DataToPersist)serializer.ReadObject(stream)!;
                Debug.WriteLine(data);
                return (
                    data.Players ?? new ObservableCollection<Player>(),
                    data.Games ?? new ObservableCollection<Game>()
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[JsonPersistency] Erreur LoadData : {ex.Message}");
                try
                {
                    File.Delete(fullPath);
                }
                catch {}

                return (
                    new ObservableCollection<Player>(),
                    new ObservableCollection<Game>()
                );
            }
        }

        public void SaveData(ObservableCollection<Player> allPlayers,ObservableCollection<Game> allGames)
        {
            try
            {
                Directory.CreateDirectory(FilePath);
                string fullPath = Path.Combine(FilePath, FileName);

                ObservableCollection<Player> existingPlayers;
                ObservableCollection<Game> existingGames;

                if (File.Exists(fullPath))
                {
                    try
                    {
                        using var readStream = File.OpenRead(fullPath);
                        var serializer = new DataContractJsonSerializer(typeof(DataToPersist));
                        var oldData = (DataToPersist)serializer.ReadObject(readStream)!;

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
                var persistSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
                var mergedData = new DataToPersist
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