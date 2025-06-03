using System.Runtime.Serialization;
namespace Models.Game
{
    /// <summary>
    /// Représente une position sur le plateau de jeu avec des coordonnées en ligne et colonne.
    /// </summary>
    [DataContract]
    public sealed class Position : IEquatable<Position>
    {
        /// <summary>
        /// Obtient le numéro de la ligne.
        /// </summary>
        [DataMember]
        public int Row { get; private set; }
        
        /// <summary>
        /// Obtient le numéro de la colonne.
        /// </summary>
        [DataMember]
        public int Column { get; private set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Position.
        /// </summary>
        /// <param name="row">Le numéro de la ligne.</param>
        /// <param name="column">Le numéro de la colonne.</param>
        public Position(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        /// <summary>
        /// Détermine si cette position est égale à une autre position.
        /// </summary>
        /// <param name="other">La position à comparer.</param>
        /// <returns>true si les positions sont égales ; sinon, false.</returns>
        public bool Equals(Position? other)
        {
            if (other is null) return false;
            return Row == other.Row && Column == other.Column;
        }

        /// <summary>
        /// Détermine si cette position est égale à un objet spécifié.
        /// </summary>
        /// <param name="obj">L'objet à comparer.</param>
        /// <returns>true si les objets sont égaux ; sinon, false.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Position);
        }

        /// <summary>
        /// Retourne le code de hachage pour cette position.
        /// </summary>
        /// <returns>Un code de hachage pour cette position.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }
    }
}