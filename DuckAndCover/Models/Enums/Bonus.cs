namespace Models.Enums
{
    /// <summary>
    /// Énumération des différents types de bonus possibles pour une carte.
    /// </summary>
    public enum Bonus
    {
        /// <summary>
        /// Aucun bonus.
        /// </summary>
        None,

        /// <summary>
        /// Bonus qui permet d'utiliser la valeur maximale de la grille.
        /// </summary>
        Max,

        /// <summary>
        /// Bonus qui permet de réutiliser le dernier numéro joué.
        /// </summary>
        Again
    }
}