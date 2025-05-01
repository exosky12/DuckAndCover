using System;
using Model;
using static System.Console;
// using Model;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            
            WriteLine("Bienvenue sur Duck&Cover");
            WriteLine("Combien de joueurs ?");

            string input = ReadLine();
            int nbJoueur;

            if (int.TryParse(input, out nbJoueur))
            {
                List<Player> players = new List<Player>();
                while(players.Count < nbJoueur) 
                {
                    WriteLine($"Pseudo du joueur numero : {players.Count + 1}");
                    string playerName = ReadLine();
                    while (playerName.Length <= 0) 
                    {
                        WriteLine("Pseudo change le pseudo");
                        playerName = ReadLine();
                    }

                    Player player = new Player(playerName);
                    players.Add(player);


                }

                foreach (Player p in players) { 
                    WriteLine(p.Name);
                    WriteLine(p.GameScore);
                    }
                






        
            }
    }
}
}