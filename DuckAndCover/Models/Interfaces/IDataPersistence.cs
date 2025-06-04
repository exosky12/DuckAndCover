using Models.Game;
using GameModel = Models.Game.Game;
using System.Collections.ObjectModel;

namespace Models.Interfaces
{
    /// <summary>
    /// Interface pour la sauvegarde et le chargement des données du jeu.
    /// </summary>
    public interface IDataPersistence
    {
        /// <summary>
        /// Charge les données du jeu depuis le fichier JSON
        /// </summary>
        /// <returns>
        /// Un tuple contenant la liste des joueurs et la liste des parties.
        /// </returns>
        (ObservableCollection<Player>, ObservableCollection<GameModel>) LoadData();

        /// <summary>
        /// Sauvegarde les données du jeu dans le fichier JSON.
        /// </summary>
        /// <param name="players">La liste des joueurs à sauvegarder.</param>
        /// <param name="games">La liste des parties à sauvegarder.</param>
        void SaveData(ObservableCollection<Player> players, ObservableCollection<GameModel> games);
    }
}