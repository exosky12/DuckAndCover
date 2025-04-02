using System;
using ClassLibrary;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            Player player = new Player("Daryl");
            Console.WriteLine($"Player Name: {player.Name}");
            Console.WriteLine($"Player Score: {player.Score}");
            player.Score = 10;
            Console.WriteLine($"Updated Player Score: {player.Score}");
            
        }
    }
}