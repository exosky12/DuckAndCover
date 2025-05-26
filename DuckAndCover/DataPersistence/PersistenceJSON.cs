using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Text.Json;
using Models.Game;
using Models.Interfaces;

namespace DataPersistence
{
    public class PersistenceJSON : IDataPersistence
    {
        public string FileName { get; set; } = "duckAndCover.json";
        
        public (ObservableCollection<Player>, ObservableCollection<Game>) LoadData()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(DataToPersist));
            DataToPersist? data = new DataToPersist();

            using (Stream s = File.OpenRead(Path.Combine(FilePath, FileName)))
            {
                data = jsonSerializer.ReadObject(s) as DataToPersist;
            }
            return (data.Players, data.Games);
        }
        
        public void SaveData(ObservableCollection<Player> players, ObservableCollection<Game> games)
        {
            if (!Directory.Exists(FilePath))
            {
                Debug.WriteLine("Directory created");
                Debug.WriteLine(Directory.GetDirectoryRoot(FilePath));
                Debug.WriteLine(FilePath);
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