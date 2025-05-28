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
            var jsonSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
            DataToPersist? data = new DataToPersist();

            using (Stream s = File.OpenRead(Path.Combine(FilePath, FileName)))
            {
                data = jsonSerializer.ReadObject(s) as DataToPersist;
            }
            return (data.Players ?? new ObservableCollection<Player>(),
                data.Games ?? new ObservableCollection<Game>());

        }
        
        public void SaveData(ObservableCollection<Player> players, ObservableCollection<Game> games)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            var jsonSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
            DataToPersist? data = new DataToPersist();

            data.Players = players;
            data.Games = games;

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
                }
            }
        }
    }
    
    
}