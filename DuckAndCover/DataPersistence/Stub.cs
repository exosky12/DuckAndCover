// File: DataPersistence/Stub.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models.Game;

namespace DataPersistence
{
    [ExcludeFromCodeCoverage]
    public sealed class Stub : IDataPersistence
    {
        public (List<Player>, List<Game>) LoadData()
        {
            
            var jordyGrid = new Grid();
            
            var cardToMove = jordyGrid.GetCard(new Position(1, 1));
            jordyGrid.RemoveCard(new Position(1, 1));
            jordyGrid.SetCard(new Position(1, 2), cardToMove);

            var jordy = new Player("Jordy", 11, new List<int> { 5, 6, 3 }, false, true, jordyGrid);
            var jules = new Player("Jules", 12, new List<int> { 4, 7, 2 }, false, false, new Grid());

           
            var onGoingGame = new Game(
                Guid.Parse("7051e963-7f97-4c11-8154-556c7af9794f"),
                new List<Player> { jordy, jules },
                currentPlayerIndex: 1,
                cardsSkipped: 0,
                isFinished: false
            );

            
            var jordy2 = new Player("Jordy", 1, new List<int> { 5, 6, 3 }, false, true, new Grid());
            var jules2 = new Player("Jules", 3, new List<int> { 4, 7, 2 }, false, false, new Grid());
            var finishedGame = new Game(
                Guid.Parse("9051e963-7f97-4c11-8154-556c7af9794f"),
                new List<Player> { jordy2, jules2 },
                currentPlayerIndex: 0,
                cardsSkipped: 8,
                isFinished: true
            );

            
            var jordyGrid1 = new Grid();
            // A) (1,1) → (2,1)
            var cardA = jordyGrid1.GetCard(new Position(1, 1));
            jordyGrid1.RemoveCard(new Position(1, 1));
            jordyGrid1.SetCard(new Position(2, 1), cardA);
            // B) (1,2) → (1,3)
            var cardB = jordyGrid1.GetCard(new Position(1, 2));
            jordyGrid1.RemoveCard(new Position(1, 2));
            jordyGrid1.SetCard(new Position(1, 3), cardB);
            // C) (2,3) → (0,0)
            var cardC = jordyGrid1.GetCard(new Position(2, 3));
            jordyGrid1.RemoveCard(new Position(2, 3));
            jordyGrid1.SetCard(new Position(0, 0), cardC);

            var jordy1 = new Player("Jordy1", 9, new List<int> { 5, 6, 3 }, false, true, jordyGrid1);

            // 5) Nouvelle grille pour jules1 avec 3 déplacements
            var julesGrid1 = new Grid();
            // D) (2,1) → (3,1)
            var cardD = julesGrid1.GetCard(new Position(2, 1));
            julesGrid1.RemoveCard(new Position(2, 1));
            julesGrid1.SetCard(new Position(3, 1), cardD);
            // E) (3,2) → (3,3)
            var cardE = julesGrid1.GetCard(new Position(3, 2));
            julesGrid1.RemoveCard(new Position(3, 2));
            julesGrid1.SetCard(new Position(3, 3), cardE);
            // F) (1,4) → (1,2)
            var cardF = julesGrid1.GetCard(new Position(1, 4));
            julesGrid1.RemoveCard(new Position(1, 4));
            julesGrid1.SetCard(new Position(1, 2), cardF);

            var jules1 = new Player("Jules1", 9, new List<int> { 4, 7, 2 }, false, false, julesGrid1);

            var crazyGame = new Game(
                Guid.Parse("5051e963-7f97-4c11-8154-556c7af9794f"),
                new List<Player> { jordy1, jules1 },
                currentPlayerIndex: 1,
                cardsSkipped: 3,
                isFinished: false
            );

            // 7) Retourne tout
            var players = new List<Player> { jordy, jules, jordy2, jules2, jordy1, jules1 };
            var games = new List<Game> { onGoingGame, finishedGame, crazyGame };
            return (players, games);
        }

        public void SaveData(List<Player> players, List<Game> games)
        {
            // No-op : stub ne persiste rien
        }
    }
}
