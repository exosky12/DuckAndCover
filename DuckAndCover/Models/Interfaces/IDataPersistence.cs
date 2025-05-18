using Models.Game;
using GameModel = Models.Game.Game;
namespace Models.Interfaces
{
    public interface IDataPersistence
    {
        (List<Player>, List<GameModel>) LoadData();

        void SaveData(List<Player> players, List<GameModel> games);
    }
}
