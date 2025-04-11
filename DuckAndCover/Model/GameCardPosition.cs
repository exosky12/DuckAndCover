namespace Model
{
    public class GameCardPosition
    {
        public GameCard Card { get; set; }
        public Position Position { get; set; }

        public GameCardPosition(GameCard card, Position position)
        {
            Card = card;
            Position = position;
        }
    }
}