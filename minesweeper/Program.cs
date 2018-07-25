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
            var boardX = SmartRead("Enter the X dimension of the board:");
            var boardY = SmartRead("Enter the Y dimension of the board:");


            // get # of mines
            var numberOfMines = SmartRead("Enter the number of mines:");
            // create board
            Gridling[][] board = new Gridling[boardX][];
            for(int i = 0; i < boardX; i++)
            {
                board[i] = new Gridling[boardY];
            }

            // place mines
            PlaceMines(ref board, numberOfMines);

            //start game loop
            while (alive) {
                // draw/update board
                DrawBoard(ref board);

                // get moves ( reveal or flag )
                Console.ReadLine();
                // end in victory or defeat
            }
            
        }

        private static void DrawBoard(ref Gridling[][] board)
        {
            var maxX = board.GetLength(0);
            var maxY = board.GetLength(1);
            Console.Clear();
            for(int y = maxY; y > -1; y--)
            {
                Console.Write(y);
                Console.Write("|");
                for(int x = 0; x < maxX + 1; x++)
                {
                    Console.Write(board[x][y].Marker);
                }
                Console.Write("\r\n");
            }
        }

        private static void PlaceMines(ref Gridling[][] board, int numberOfMines)
        {
            Random rnd = new Random();
            var maxX = board.GetLength(0);
            var maxY = board[0].GetLength(0);

            while (numberOfMines > 0)
            {
                var localX = rnd.Next(0, maxX);
                var localY = rnd.Next(0, maxY);
                if (!board[localX][localY].Bomb)
                {
                    board[localX][ localY].Bomb = true;
                    AlertNeighbors(ref board, localX, localY);
                    numberOfMines--;
                }
            }
        }

        private static void AlertNeighbors(ref Gridling[][] board, int localX, int localY)
        {
            var maxX = board[0].GetLength(0)-1;
            var maxY = board.GetLength(1)-1;
            
            //bottom left; x-1 y-1
            if (localX - 1 > 0 && localY - 1 > 0 )
            {
                board[localX - 1][localY - 1].DangerousNeighbors++;
            }

            //left; x-1 y
            if (localX - 1 > 0)
            {
                board[localX - 1][localY].DangerousNeighbors++;
            }

            //upper left; x-1 y+1
            if (localX - 1 < 0 && localY + 1 < maxY )
            {
                board[localX - 1][localY + 1].DangerousNeighbors++;
            }

            //top; y+1
            if (localY + 1 < maxY)
            {
                board[localX][localY + 1].DangerousNeighbors++;
            }

            //upper right; x+1 y+1
            if (localX + 1 < maxX && localY + 1 < maxY)
            {
                board[localX + 1][localY + 1].DangerousNeighbors++;
            }

            //right; x+1
            if(localX + 1 < maxX)
            {
                board[localX + 1][localY].DangerousNeighbors++;
            }

            //lower right; x+1 y-1
            if(localX + 1 < maxX && localY - 1 > 0)
            {
                board[localX + 1][localY - 1].DangerousNeighbors++;
            }

            //bottom; y-1
            if(localY - 1 > 0)
            {
                board[localX][localY - 1].DangerousNeighbors++;
            }
        }

        public static int SmartRead(string message)
        {
            int result;
            Console.Write(message);
            var Value = Console.ReadLine();
            if(Int32.TryParse(Value, out result))
            {
                if(result < 1)
                {
                    Console.WriteLine("Value must be an integer greater than 0.");
                    return SmartRead(message);
                }
                return result;
            }
            else
            {
                Console.WriteLine("Value must be an integer greater than 0.");
                return SmartRead(message);
            }
        }


    }

    public class Gridling
    {
        public bool Flagged { get; set; }
        public bool Revealed { get; set; }
        public bool Bomb { get; set; }
        public int DangerousNeighbors { get; set; }
        public char Marker { get
            {
                if (this.Flagged) return 'F';
                if (this.Revealed) return (char)this.DangerousNeighbors;
                return '■';
            } }

        public Gridling()
        {
            Flagged = false;
            Revealed = false;
            Bomb = false;
            DangerousNeighbors = 0;
        }
    }
}
