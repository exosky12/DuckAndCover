using System.Collections.ObjectModel;
using Models.Game;
namespace DataPersistence
{
    public class DataToPersist
    {
        public required ObservableCollection<Player> Players { get; set; }

        public required ObservableCollection<Game> Games { get; set; }


    }
}
