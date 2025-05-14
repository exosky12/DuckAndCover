namespace Model;

public class Player
{
    public string Name { get; init; }

    public bool HasPassed { get; set; }
    public bool HasPlayed { get; set; }

    public List<int> Scores { get; }

    public int TotalScore => Scores.Sum();

    public int StackCounter { get; private set; }

    public Player(string name)
    {
        this.StackCounter = 0;
        this.Scores = new List<int>();
        this.Name = name;
        this.HasPassed = false;
        this.HasPlayed = false;
    }
    
    public Grid Grid { get; } = new Grid();

    public bool Cover(GameCard aboveCard, GameCard belowCard, Grid grid, Game game)
    {
        if (game.Rules.IsValidMove(aboveCard.Position, belowCard.Position, grid, "cover"))
        {
            int aboveIndex = grid.GameCardsGrid.FindIndex(c => c.Position.Row == aboveCard.Position.Row && c.Position.Column == aboveCard.Position.Column);
            int belowIndex = grid.GameCardsGrid.FindIndex(c => c.Position.Row == belowCard.Position.Row && c.Position.Column == belowCard.Position.Column);

            if (aboveIndex >= 0 && belowIndex >= 0)
            {
                aboveCard.Position = new Position(belowCard.Position.Row, belowCard.Position.Column);
                grid.GameCardsGrid[belowIndex] = aboveCard;
                grid.GameCardsGrid.RemoveAt(aboveIndex > belowIndex ? aboveIndex + 1 : aboveIndex);
            }

            game.NextPlayer();
            StackCounter = grid.GameCardsGrid.Count;
            return true;
        }
        else
        {
            return false;
        }
    }

    
    public bool Duck(GameCard card, Position newPos, Grid grid, Game game)
    {
        var (isAdjacent, adjacentCard) = grid.IsAdjacentToCard(newPos);
    
        if (isAdjacent && card != adjacentCard)
        {
            grid.RemoveCard(card.Position);
        
            grid.SetCard(newPos, card);
            game.NextPlayer();
            StackCounter = grid.GameCardsGrid.Count;
            return true;
        }

        return false;
    }
    public void CallCoin(Game game, Grid grid)
    {   
        game.NextPlayer();
        StackCounter = grid.GameCardsGrid.Count;
        /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer donc on dit "coin" */
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