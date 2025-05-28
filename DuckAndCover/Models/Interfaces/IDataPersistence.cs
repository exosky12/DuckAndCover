using Models.Game;
using GameModel = Models.Game.Game;
using System.Collections.ObjectModel;
namespace Models.Interfaces
{
    public interface IDataPersistence
    {
        (ObservableCollection<Player>, ObservableCollection<GameModel>) LoadData();

        void SaveData(ObservableCollection<Player> players, ObservableCollection<GameModel> games);
    }
}
