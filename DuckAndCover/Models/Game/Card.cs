using System.Runtime.Serialization;

namespace Models.Game
{
    [DataContract]
    public abstract class Card
    {
        [DataMember]
        public int Number { get; set; }

        protected Card(int number)
        {
            this.Number = number;
        }
    }
}