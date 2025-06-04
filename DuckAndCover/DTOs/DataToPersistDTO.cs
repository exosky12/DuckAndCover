using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Models.Game;

namespace DTOs
{
    [KnownType(typeof(Bot))]
    [DataContract]
    public class DataToPersistDto
    {
        [DataMember]
        public ObservableCollection<Player>? Players { get; set; }

        [DataMember]
        public ObservableCollection<Game>? Games { get; set; }
    }
}