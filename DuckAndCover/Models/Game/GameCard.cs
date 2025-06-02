using System.Runtime.Serialization;

namespace Models.Game
{
    [DataContract]
    public class GameCard : Card
    {
        [DataMember]
        public Position Position { get; set; }
        
        [DataMember]
        public int Splash { get; set; }


        public GameCard(int splash, int number) : base(number)
        {
            this.Position = new Position(0, 0);
            this.Splash = splash;
        }
    }
}