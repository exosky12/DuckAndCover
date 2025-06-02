using System.Runtime.Serialization;
namespace Models.Game
{
    [DataContract]
    public sealed class Position : IEquatable<Position>
    {
        [DataMember]
        public int Row { get; private set; }
        
        [DataMember]
        public int Column { get; private set; }

        public Position(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }


        public bool Equals(Position? other)
        {
            if (other is null) return false;
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Position);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }
    }
}