using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public class Square
    {
        //chess specific position coordinates for move history
        public int col; //1->8, will be stringified to A->H
        public int row; //1->8
        public Piece? piece; //nullable for empty square
        public Square(int column, int row)
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

        public string getSquareColor()
        {
            if (IsBlackSquare())
                return "Black";
            else
                return "White";
        }

        public string get_piece_color()
        {
            if (piece == null)
                return "no piece here";
            else
                return piece.color.ToString();
        }
        public override string ToString()
        {
            char colLetter = (char)('A' + (col - 1));
            return $"{colLetter}{row}";
        }
    }
}
