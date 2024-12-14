using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    internal class Board
    {
        public Square[,] grid;
        public Board()
        {
            grid = new Square[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    char columnLetter = (char)('A' + col);
                    grid[row, col] = new Square(columnLetter.ToString(), row + 1);
                }
            }
        }   
    }
}
