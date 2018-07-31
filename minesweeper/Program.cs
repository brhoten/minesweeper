using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    class MineSweeper
    {
        static void Main(string[] args)
        {
            bool alive = true;
            // get board dimensions
            var boardX = SmartRead("\r\nEnter the X dimension of the board (1-9):");
            var boardY = SmartRead("\r\nEnter the Y dimension of the board (1-9):");


            // get # of mines
            var numberOfMines = SmartRead("\r\nEnter the number of mines (1-9):");
            // create board
            Gridling[,] board = new Gridling[boardY,boardX];
            for(int i = 0; i < boardY; i ++)
            {
                for(int j = 0; j < boardX; j++)
                {
                    board[i, j] = new Gridling(j,i);
                }
            }

            // place mines
            PlaceMines(ref board, numberOfMines);

            //start game loop
            while (alive) {
                // draw/update board
                DrawBoard(ref board);

                // get moves ( quit, reveal,  or un/flag )
                var input = GetMove();
                // end in victory or defeat
                Gridling spot = new Gridling(0,0);
                switch (input)
                {
                    case ConsoleKey.Q:
                        Console.WriteLine("\r\nQuitter.");
                        alive = false;
                        break;
                    case ConsoleKey.R:
                        spot = GetCoordinates(ref board);
                        if (spot.Bomb && !spot.Flagged)
                        {
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

                            RevealSpot(ref board, spot);
                            alive = StillPlaying(ref board);
                        }
                        break;
                    case ConsoleKey.U:
                        spot = GetCoordinates(ref board);
                        if (spot.Flagged)
                        {
                            board[spot.Y, spot.X].Flagged = false;
                        }
                        else
                        {
                            Console.WriteLine("\r\nNot a flagged spot.");
                        }
                        break;
                    case ConsoleKey.F:
                        spot = GetCoordinates(ref board);
                        if (spot.Flagged)
                        {
                            Console.WriteLine("\r\nSpot already flagged.");

                        }
                        else
                        {
                            board[spot.Y, spot.X].Flagged = true;
                            alive = StillPlaying(ref board);
                        }
                        break;
                    default: continue;
                }

            }

            DrawBoard(ref board);
            Console.WriteLine("\r\nThanks for playing.");
            Console.ReadLine();
        }

        private static void RevealSpot(ref Gridling[,] board, Gridling spot)
        {
            board[spot.Y, spot.X].Revealed = true;
            if (spot.DangerousNeighbors == 0)
            {
                var neighbors = FindMyNeighbors(ref board, spot);
                foreach (var neighbor in neighbors.Where(x => !x.Revealed))
                {
                    RevealSpot(ref board, neighbor);
                }
            }
        }

        private static bool StillPlaying(ref Gridling[,] board)
        {
            return board.Cast<Gridling>().ToList().Any(x => !x.Bomb && !x.Revealed);
        }

        private static Gridling GetCoordinates(ref Gridling[,] board)
        {
            int x,y;
            bool success;

            Console.Write("\r\nEnter X coordinate:");
            success = Int32.TryParse(Console.ReadKey().KeyChar.ToString(), out x);
            if (!success) return GetCoordinates(ref board);

            Console.Write("\r\nEnter Y coordinate:");
            success = Int32.TryParse(Console.ReadKey().KeyChar.ToString(), out y);
            if (!success) return GetCoordinates(ref board);

            return board[y, x];
        }


        private static ConsoleKey GetMove()
        {
            Console.Write("You can (q)uit, (f)lag or (u)nflag a space, or (r)eveal a space: ");
            var result = Console.ReadKey().Key;
            if (result == ConsoleKey.Q || result == ConsoleKey.F || result == ConsoleKey.U || result == ConsoleKey.R) return result;
            return GetMove();
        }

        private static void DrawBoard(ref Gridling[,] board)
        {
            var maxY = board.GetLength(0)-1;
            var maxX = board.GetLength(1);
            Console.WriteLine();
            for(int y = maxY; y > -1; y--)
            {
                Console.Write(y);
                Console.Write("|");
                for(int x = 0; x < maxX; x++)
                {
                    Console.Write(board[y, x].Marker);
                }
                Console.Write("\r\n");
            }
            Console.Write(" +");
            for (int i = 0; i < maxX; i++) Console.Write("-");
            Console.Write("\r\n  ");
            for (int i = 0; i < maxX; i++) Console.Write(i);
            Console.Write("\r\n");
        }

        private static void PlaceMines(ref Gridling[,] board, int numberOfMines)
        {
            Random rnd = new Random();
            var maxY = board.GetLength(0);
            var maxX = board.GetLength(1);

            while (numberOfMines > 0)
            {
                var localX = rnd.Next(0, maxX);
                var localY = rnd.Next(0, maxY);
                if (!board[localY,localX].Bomb)
                {
#if DEBUG
                    Console.WriteLine("\r\nPlanting bomb at {0},{1}", localX, localY);
#endif
                    board[localY, localX].Bomb = true;
                    AlertNeighbors(ref board, board[localY, localX]);
                    numberOfMines--;
                }
            }
        }

        private static List<Gridling> FindMyNeighbors(ref Gridling[,] board, Gridling spot)
        {
            var results = new List<Gridling>();
            var localX = spot.X;
            var localY = spot.Y;
            var maxY = board.GetLength(0);
            var maxX = board.GetLength(1);

            if (localX - 1 > -1 && localY - 1 > -1)
            {
                results.Add(board[localY - 1, localX - 1]);
            }

            //left; x-1 y
            if (localX - 1 > -1)
            {
                results.Add(board[localY, localX - 1]);
            }

            //upper left; x-1 y+1
            if (localX - 1 > -1 && localY + 1 < maxY)
            {
                results.Add(board[localY + 1, localX - 1]);
            }

            //top; y+1
            if (localY + 1 < maxY)
            {
                results.Add(board[localY + 1, localX]);
            }

            //upper right; x+1 y+1
            if (localX + 1 < maxX && localY + 1 < maxY)
            {
                results.Add(board[localY + 1, localX + 1]);
            }

            //right; x+1
            if (localX + 1 < maxX)
            {
                results.Add(board[localY, localX + 1]);
            }

            //lower right; x+1 y-1
            if (localX + 1 < maxX && localY - 1 > -1)
            {
                results.Add(board[localY - 1, localX + 1]);
            }

            //bottom; y-1
            if (localY - 1 > -1)
            {
                results.Add(board[localY - 1, localX]);
            }

            return results;
        }



        private static void AlertNeighbors(ref Gridling[,] board, Gridling spot)
        {
            var list = FindMyNeighbors(ref board, spot);
            foreach(var gridling in list)
            {
                gridling.DangerousNeighbors++;
            }
        }

        public static int SmartRead(string message)
        {
            int result;
            Console.Write(message);
            var Value = Console.ReadKey();
            if(Int32.TryParse(Value.KeyChar.ToString(), out result))
            {
                if(result < 1 || result > 9)
                {
                    Console.WriteLine("\r\nValue must be an integer between 1 and 9.");
                    return SmartRead(message);
                }
                return result;
            }
            else
            {
                Console.WriteLine("\r\nValue must be an integer between 1 and 9.");
                return SmartRead(message);
            }
        }


    }

    public class Gridling
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Flagged { get; set; }
        public bool Revealed { get; set; }
        public bool Bomb { get; set; }
        public int DangerousNeighbors { get; set; }
        public char Marker { get
            {
                if (this.Flagged) return 'F';
                if (this.Revealed) return Convert.ToChar(this.DangerousNeighbors.ToString());
                return '■';
            } }

        public Gridling(int x, int y)
        {
            X = x;
            Y = y;
            Flagged = false;
            Revealed = false;
            Bomb = false;
            DangerousNeighbors = 0;
        }
    }
}
