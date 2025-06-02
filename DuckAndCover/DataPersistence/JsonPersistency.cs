using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
                // Pas de fichier : on retourne des listes vides
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
                return (
                    data.Players ?? new ObservableCollection<Player>(),
                    data.Games ?? new ObservableCollection<Game>()
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[JsonPersistency] Erreur LoadData : {ex.Message}");
                // Si JSON corrompu ou autre, on efface le fichier pour repartir propre
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

        public void SaveData(
     ObservableCollection<Player> allPlayers,
     ObservableCollection<Game> allGames)
        {
            try
            {
                Directory.CreateDirectory(FilePath);
                string fullPath = Path.Combine(FilePath, FileName);

                // 1) Charger l’ancien contenu (s’il existe)
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

                // 2) Fusionner "allPlayers" dans "existingPlayers" (par Name)
                foreach (var newPlayer in allPlayers)
                {
                    var match = existingPlayers.FirstOrDefault(p => p.Name == newPlayer.Name);
                    if (match == null)
                    {
                        // Joueur entièrement nouveau → on l'ajoute
                        existingPlayers.Add(newPlayer);
                    }
                    else
                    {
                        // Joueur déjà présent → on ajoute seulement les scores qui n'existent pas encore
                        foreach (var score in newPlayer.Scores)
                        {
                            if (!match.Scores.Contains(score))
                                match.Scores.Add(score);
                        }
                    }
                }

                // 3) Fusionner "allGames" dans "existingGames" (par Id)
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
                        // Partie déjà présente : on met à jour uniquement les champs pertinents
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