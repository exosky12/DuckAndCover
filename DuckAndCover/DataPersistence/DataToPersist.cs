using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Models.Game;
namespace DataPersistence
{
    [DataContract]
    public class DataToPersist
    {
        [DataMember]
        public ObservableCollection<Player>? Players { get; set; }

        [DataMember]
        public ObservableCollection<Game>? Games { get; set; }
    }
}
