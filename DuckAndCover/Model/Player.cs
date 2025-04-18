namespace Model;

public class Player
{
    public string Name { get; init; }

    public int GameScore { get; set; }

    public int TotalScore { get; set; }

    public int StackCounter { get; set; }

    public Player(string name)
    {
        this.Name = name;
    }

    public void Cover(GameCard aboveCard, GameCard belowCard)
    {
        /* ADD logic */
    }

    public void Duck(GameCard cardToMove, Position position)
    {
        /* ADD logic */
    }

    public bool CanPlay(Game game)
    {
        /* Va demander aux règles si dans le cas présent le joueur à un ou plusieurs choix de jeu */
        /* SI AUCUNES POSSIBILITÉS --> appeler NextPlayer dans game puis appeler la méthode CallCoin de Player */
    }

    public void CallCoin()
    {
        /* FAIRE lien entre modèles et vues pour afficher quelque chose à l'écran comme quoi on peut pas jouer donc on dit "coin" */
    }
}