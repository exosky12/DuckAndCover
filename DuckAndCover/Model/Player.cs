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
    
    public Grid Grid { get; set; } = new Grid();

    public void Cover(GameCard aboveCard, GameCard belowCard, Grid grid, Game game)
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
        }
        else
        {
            game.NextPlayer();
        }
    }

    
    public void Duck(GameCard card, Position newPos, Grid grid, Game game)
    {
        if (grid.GetCard(newPos) != null)
        {
            Console.WriteLine("Position déjà occupée, mouvement invalide.");
            game.NextPlayer();
            return;
        }

        var existingCard = grid.GetCard(card.Position);
        if (existingCard == null || existingCard != card)
        {
            Console.WriteLine("La carte sélectionnée n'est pas à la position indiquée.");
            game.NextPlayer();
            return;
        }

        bool hasAdjacentCard = Grid.IsAdjacentToCard(newPos);
    
        if (hasAdjacentCard)
        {
            grid.RemoveCard(card.Position);
        
            card.Position = newPos;
        
            grid.SetCard(newPos, card);
        
            Console.WriteLine($"Carte déplacée avec succès à la position ({newPos.Row + 1}, {newPos.Column + 1})");
        }
        else
        {
            Console.WriteLine("Aucune carte adjacente à la nouvelle position, mouvement invalide.");
            game.NextPlayer();
        }
    }
    public void CallCoin()
    {
        /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer donc on dit "coin" */
    }
}