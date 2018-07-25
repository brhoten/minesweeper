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
            // get board dimensions
            var boardX = SmartRead("Enter the X dimension of the board:");
            var boardY = SmartRead("Enter the Y dimension of the board:");


            // get # of mines
            var numberOfMines = SmartRead("Enter the number of mines:");
            Console.WriteLine("boardX {0}, boardY {1}, mines {2}", boardX, boardY, numberOfMines);
            Console.ReadLine();
            // place mines
            // draw/update board
            // get moves ( reveal or flag )
            // end in victory or defeat
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
}
