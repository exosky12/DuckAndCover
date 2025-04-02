namespace ClassLibrary
{
    public class Player
    {
        string name;
        int score;
        // Ajouter attribut Grille

        public Player(string name)
        {
            this.name = name;
            score = 0;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
    }
}