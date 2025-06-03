using System.Runtime.Serialization;

namespace Models.Game
{
    [DataContract]
    public class Player
    {

        [DataMember]
        public string Name { get; init; }
        
        [DataMember]
        public bool HasSkipped { get; set; }
        
        [DataMember]
        public bool HasPlayed { get; set; }
        
        [DataMember]
        public List<int> Scores { get; set; } = new List<int>();
        
        public int TotalScore => Scores.Sum();

        [DataMember]
        public int StackCounter { get; set; }

        [IgnoreDataMember]
        public Grid Grid { get; private set; } = new Grid();

        public Player(string name)
        {
            this.StackCounter = 12;
            this.Scores = new List<int>();
            this.Name = name;
            this.HasSkipped = false;
            this.HasPlayed = false;
        }

        public Player(string name,int stack, List<int> scores,bool skipped,bool played,Grid grid)
        {
            this.Name = name;
            this.StackCounter = stack ;
            this.Scores = new List<int>();
            this.HasSkipped=skipped;    
            this.HasPlayed=played;
            this.Grid = grid;
        }
        
        public bool HasCardWithNumber(int number)
        {
            foreach (var card in Grid.GameCardsGrid)
            {
                if (card.Number == number)
                    return true;
            }

            return false;
        }
    }
}