using System;

namespace MineSweeper
{
    public class Cell
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

        public Cell(int x, int y)
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