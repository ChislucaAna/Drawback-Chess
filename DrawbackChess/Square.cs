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
        public string column; //A->H
        public int row; //1->8
        public Piece? piece; //nullable for empty square
        public Square(string column, int row)
        {
            this.column = column;
            this.row = row;
        }
        public bool is_occupied()
        {
            return piece != null;
        }
    }
}
