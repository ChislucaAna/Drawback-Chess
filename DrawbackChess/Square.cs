using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public class Square
    {
        //chess specific coordinates for move history
        public char col; //A->H
        public int row; //1->8
        public Piece? piece; //nullable for empty square
        public Square(char column, int row)
        {
            this.col = column;
            this.row = row;
        }
        public bool is_occupied()
        {
            return piece != null;
        }
        public bool IsBlackSquare()
        {
            return (row + col) % 2 != 0;
        }
    }
}
