using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes.GameClasses;
using DrawbackChess.Components.Pages;

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

        public Square(string position)
        {
            this.col = position[0] - 'a' + 1;
            this.row = position[1];
        }
        public bool is_occupied()
        {
            return piece != null;
        }
        public bool IsBlackSquare()
        {
            return (row + col) % 2 == 0;
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
            char colLetter = (char)('a' + (col - 1));
            return $"{colLetter}{row}";
        }

        public bool IsDangerousForKing(string color, Board board)
        {
            foreach (var square in board.grid)
            {
                if (square == null)
                    continue;

                var piece = square.piece;
                if (piece != null && piece.color != color)
                {
                    var chessRange = piece.GetChessRange(square,board);
                    if (chessRange?.Contains(this) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
