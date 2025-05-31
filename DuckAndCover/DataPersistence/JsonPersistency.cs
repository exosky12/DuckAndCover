using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;
using Models.Game;
using Models.Interfaces;

namespace DataPersistence
{
    public class JsonPersistency : IDataPersistence
    {
        public string FileName { get; set; } = "duckAndCover_data.json";
        public string FilePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DuckAndCover");
        
        public (ObservableCollection<Player>, ObservableCollection<Game>) LoadData()
        {
            Debug.WriteLine($"Attempting to load data from: {Path.Combine(FilePath, FileName)}");
            var jsonSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
            DataToPersist? data = new DataToPersist();

            try
            {
                using (Stream s = File.OpenRead(Path.Combine(FilePath, FileName)))
                {
                    data = jsonSerializer.ReadObject(s) as DataToPersist;
                    Debug.WriteLine($"Successfully loaded data: {data.Players?.Count ?? 0} players, {data.Games?.Count ?? 0} games");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }

            return (data.Players ?? new ObservableCollection<Player>(),
                data.Games ?? new ObservableCollection<Game>());
        }
        
        public void SaveData(ObservableCollection<Player> players, ObservableCollection<Game> games)
        {
            Debug.WriteLine($"Attempting to save data: {players.Count} players, {games.Count} games");
            
            if (!Directory.Exists(FilePath))
            {
                Debug.WriteLine($"Creating directory at: {FilePath}");
                Directory.CreateDirectory(FilePath);
            }

            var jsonSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
            DataToPersist? data = new DataToPersist();

            data.Players = players;
            data.Games = games;

            try
            {
                var settings = new XmlWriterSettings() { Indent = true };
                using (FileStream stream = File.Create(Path.Combine(FilePath, FileName)))
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                               stream,
                               Encoding.UTF8,
                               false,
                               true))
                    {
                        jsonSerializer.WriteObject(writer, data);
                        writer.Flush();
                        Debug.WriteLine("Data successfully saved to file");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving data: {ex.Message}");
                throw;
            }
        }
    }
    
    
}