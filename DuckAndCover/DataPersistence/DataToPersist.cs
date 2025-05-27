using System.Collections.ObjectModel;
using Models.Game;
namespace DataPersistence
{
    public class DataToPersist
    {
        public ObservableCollection<Player>? Players { get; set; }

        public ObservableCollection<Game>? Games { get; set; }


    }
}
