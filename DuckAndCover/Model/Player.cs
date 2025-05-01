namespace Model;

public class Player
{
    public string Name { get; init; }

    public int GameScore { get; set; }

    public List<int> Scores { get; set; }

    public int TotalScore => Scores.Sum();

    public int StackCounter { get; set; }

    public Player(string name)
    {
        this.GameScore = 0;
        this.StackCounter = 0;
        this.Scores = new List<int>();
        this.Name = name;
    }

    public void Cover(GameCard aboveCard, GameCard belowCard, Grid grid, Game game)
    {
        if (game.Rules.IsValidMove(aboveCard.Position, belowCard.Position, grid, "cover"))
        {
            /* ADD logic */
            aboveCard.Position = belowCard.Position;
            grid.RemoveCard(belowCard.Position);
        }
        else
        {
            /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer */
            game.NextPlayer();
        }
    }

    public void Duck(GameCard cardToMove, Position position, Grid grid, Game game)
    {
        if (game.Rules.IsValidMove(cardToMove.Position, position, grid, "duck"))
        {
            /* ADD logic */
            grid.SetCard(position, cardToMove);
        }
        else
        {
            /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer */
            game.NextPlayer();
        }
    }

    public void CallCoin()
    {
        /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer donc on dit "coin" */
    }
}