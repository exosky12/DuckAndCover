namespace Models.Interfaces
{
    /// <summary>
    /// Interface pour un générateur d'objets de type T.
    /// </summary>
    /// <typeparam name="T">Le type des objets générés.</typeparam>
    public interface IGenerator<T>
    {
        /// <summary>
        /// Génère une liste d'objets de type T.
        /// </summary>
        /// <returns>La liste des objets générés.</returns>
        List<T> Generate();
    }
}