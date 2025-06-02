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

        // Par exemple : %AppData%\DuckAndCover\duckAndCover_data.json
        public string FilePath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DuckAndCover"
            );

        /// <summary>
        /// Lit l’historique existant (players + games) si le fichier existe.
        /// Sinon retourne deux collections vides.
        /// </summary>
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
                catch { /* on ignore */ }

                return (
                    new ObservableCollection<Player>(),
                    new ObservableCollection<Game>()
                );
            }
        }

        /// <summary>
        /// Charge l’ancien fichier, fusionne les collections passées (allPlayers, allGames)
        /// avec les données existantes, sans dupliquer ni écraser inutilement, puis 
        /// écrase le fichier avec le résultat fusionné.
        /// </summary>
        public void SaveData(
            ObservableCollection<Player> allPlayers,
            ObservableCollection<Game> allGames)
        {
            try
            {
                Directory.CreateDirectory(FilePath);
                string fullPath = Path.Combine(FilePath, FileName);

                // (1) Charger l’ancien contenu
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
                        Debug.WriteLine($"[JsonPersistency] Erreur lecture antérieure : {ex.Message}");
                        // Si corrompu, on repart sur zéro
                        existingPlayers = new ObservableCollection<Player>();
                        existingGames = new ObservableCollection<Game>();
                    }
                }
                else
                {
                    existingPlayers = new ObservableCollection<Player>();
                    existingGames = new ObservableCollection<Game>();
                }

                // (2) Fusionner allPlayers dans existingPlayers par clé (Name)
                foreach (var newPlayer in allPlayers)
                {
                    var match = existingPlayers.FirstOrDefault(p => p.Name == newPlayer.Name);
                    if (match == null)
                    {
                        // Nouveau joueur → on l’ajoute
                        existingPlayers.Add(newPlayer);
                    }
                    else
                    {
                        // Joueur existant → on ajoute ses scores récents
                        foreach (var score in newPlayer.Scores)
                        {
                            if (!match.Scores.Contains(score))
                                match.Scores.Add(score);
                        }
                        // On peut synchroniser d’autres champs si besoin
                    }
                }

                // (3) Fusionner allGames dans existingGames par clé (Id)
                foreach (var newGame in allGames)
                {
                    var match = existingGames.FirstOrDefault(g => g.Id == newGame.Id);
                    if (match == null)
                    {
                        // Nouvelle partie → on l’ajoute
                        existingGames.Add(newGame);
                    }
                    else
                    {
                        // Partie existante → on met à jour seulement les champs modifiables
                        match.IsFinished = newGame.IsFinished;
                        // Si vous stockez LastNumber, CurrentDeckCard, etc. :
                        match.LastNumber = newGame.LastNumber;
                        match.CardsSkipped = newGame.CardsSkipped;
                        match.LastGameFinishStatus = newGame.LastGameFinishStatus;
                        // Si vous voulez mettre à jour la grille complète :
                        match.Players = newGame.Players;
                        match.Deck = newGame.Deck;
                        // etc., selon ce que vous jugez nécessaire de rafraîchir
                    }
                }

                // (4) Écrire le JSON fusionné dans le fichier (en écrasant l’ancien contenu)
                using var writeStream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
                var persistSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
                var mergedData = new DataToPersist
                {
                    Players = existingPlayers,
                    Games = existingGames
                };
                persistSerializer.WriteObject(writeStream, mergedData);
                writeStream.Flush();

                Debug.WriteLine($"[JsonPersistency] Données fusionnées et sauvegardées dans {fullPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[JsonPersistency] Erreur SaveData : {ex.Message}");
                throw;
            }
        }
    }
}
