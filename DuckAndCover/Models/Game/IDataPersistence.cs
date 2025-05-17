using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Game
{
    public interface IDataPersistence
    {
        (List<Player>, List<Game>) LoadData();

        void SaveData(List<Player> players, List<Game> games);
    }
}
