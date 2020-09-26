using System;
using System.Collections.Generic;
using System.Linq;
using static System.Int32;

namespace MineSweeper
{
    internal class MineSweeper
    {

        private static void Main()
        {
            // start alive
            var alive = true;

            // get board dimensions
            var boardX = SafeRead("\r\nEnter the X dimension of the board (1-9):");
            var boardY = SafeRead("\r\nEnter the Y dimension of the board (1-9):");

            // get # of mines
            var numberOfMines = SafeRead("\r\nEnter the number of mines (1-9):");
            // create board
            var board = new Cell[boardY, boardX];
            for (var i = 0; i < boardY; i++)
            for (var j = 0; j < boardX; j++)
                board[i, j] = new Cell(j, i);

            // place mines
            PlaceMines(board, numberOfMines);

            //start game loop
            while (alive)
            {
                // draw/update board
                DrawBoard(board);

                // get moves ( quit, reveal,  or un/flag )
                var input = GetMove();
                // end in victory or defeat
                Cell spot;
                switch (input)
                {
                    case ConsoleKey.Q:
                        Console.WriteLine("\r\nQuitter.");
                        alive = false;
                        break;
                    case ConsoleKey.R:
                        spot = GetCoordinates(board);
                        if (spot.Bomb && !spot.Flagged)
                        {
                            DrawBoard(board);
                            Console.WriteLine("\r\nDead.");
                            alive = false;
                            continue;
                        }

                        if (spot.Flagged)
                        {
                            Console.WriteLine("\r\nCannot reveal a flagged spot.");
                        }
                        else
                        {
                            RevealSpot(board, spot);
                            alive = StillPlaying(board);
                        }

                        break;
                    case ConsoleKey.U:
                        spot = GetCoordinates(board);
                        if (spot.Flagged)
                            board[spot.Y, spot.X].Flagged = false;
                        else
                            Console.WriteLine("\r\nNot a flagged spot.");
                        break;
                    case ConsoleKey.F:
                        spot = GetCoordinates(board);
                        if (spot.Flagged)
                        {
                            Console.WriteLine("\r\nSpot already flagged.");
                        }
                        else
                        {
                            board[spot.Y, spot.X].Flagged = true;
                            alive = StillPlaying(board);
                        }

                        break;
                    default: continue;
                }
            }

            Console.WriteLine("\r\nThanks for playing.");
            Console.ReadLine();
        }

        private static void RevealSpot(Cell[,] board, Cell spot)
        {
            board[spot.Y, spot.X].Revealed = true;
            if (spot.DangerousNeighbors != 0) return;
            var neighbors = FindMyNeighbors(board, spot);
            foreach (var neighbor in neighbors.Where(x => !x.Revealed)) RevealSpot(board, neighbor);
        }

        private static bool StillPlaying(Cell[,] board)
        {
            return board.Cast<Cell>().ToList().Any(x => !x.Bomb && !x.Revealed);
        }

        private static Cell GetCoordinates(Cell[,] board)
        {
           var x = SafeRead("Enter X coordinate:");
           var y = SafeRead("Enter Y coordinate:");
           return board[y-1, x-1];
        }

        private static ConsoleKey GetMove()
        {
            while (true)
            {
                Console.Write("You can (q)uit, (f)lag or (u)nflag a space, or (r)eveal a space: ");
                var result = Console.ReadKey().Key;
                if (result == ConsoleKey.Q || result == ConsoleKey.F || result == ConsoleKey.U ||
                    result == ConsoleKey.R) return result;
            }
        }

        private static void DrawBoard(Cell[,] board)
        {
            Console.Clear();
            var maxY = board.GetLength(0);
            var maxX = board.GetLength(1);
            Console.WriteLine();
            for (var y = maxY; y > 0; y--)
            {
                Console.Write(y);
                Console.Write("|");
                for (var x = 1; x <= maxX; x++) Console.Write(board[y-1, x-1].Marker);
                Console.Write("\r\n");
            }

            Console.Write("Y+");
            for (var i = 0; i < maxX; i++) Console.Write("-");
            Console.Write(Environment.NewLine);
            Console.Write(" X");
            for (var i = 1; i <= maxX; i++) Console.Write(i);
            Console.Write(Environment.NewLine);
        }

        private static void PlaceMines(Cell[,] board, int numberOfMines)
        {
            var rnd = new Random();
            var maxY = board.GetLength(0);
            var maxX = board.GetLength(1);

            while (numberOfMines > 0)
            {
                var localX = rnd.Next(0, maxX);
                var localY = rnd.Next(0, maxY);
                if (board[localY, localX].Bomb) continue;
#if DEBUG
                Console.WriteLine("\r\nPlanting bomb at {0},{1}", localX, localY);
#endif
                board[localY, localX].Bomb = true;
                AlertNeighbors(board, board[localY, localX]);
                numberOfMines--;
            }
        }

        // https://www.royvanrijn.com/blog/2019/01/longest-path/#neighbors-algorithm
        private static IEnumerable<Cell> FindMyNeighbors(Cell[,] board, Cell spot)
        {
            var results = new List<Cell>();

            for (var direction = 0; direction < 9; direction++)
            {
                if (direction == 4) continue;

                var x = spot.X + (direction % 3 - 1);
                var y = spot.Y + (direction / 3 - 1);

                if (x >= 0 && x < board.GetLength(1) && y >= 0 && y < board.GetLength(0)) results.Add(board[y, x]);
            }

            return results;
        }

        private static void AlertNeighbors(Cell[,] board, Cell spot)
        {
            var neighbors = FindMyNeighbors(board, spot);
            foreach (var neighbor in neighbors) neighbor.DangerousNeighbors++;
        }

        public static int SafeRead(string message)
        {
            while (true)
            {
                Console.Write(Environment.NewLine + message);
                var value = Console.ReadKey();
                if (TryParse(value.KeyChar.ToString(), out var result))
                    if (result >= 1 && result <= 9)
                        return result;

                Console.WriteLine("\r\nValue must be an integer between 1 and 9.");
            }
        }
    }
}