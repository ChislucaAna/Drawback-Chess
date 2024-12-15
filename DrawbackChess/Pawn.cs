using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    internal class Pawn:Piece
    {
        public Pawn(string color) : base(color, "Pawn") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare, Board board)
        {
            var possibleMoves = new HashSet<Square>();

            int direction = (color == "White") ? 1 : -1; // White moves up, Black moves down
            int row = currentSquare.row;
            int col = currentSquare.col;

            // Forward move
            var forwardSquare = board.GetSquareAt(row + direction, col);
            if (forwardSquare != null && forwardSquare.piece == null)
            {
                possibleMoves.Add(forwardSquare);
            }

            // Diagonal capture
            foreach (var offset in new[] { -1, 1 })
            {
                var diagonalSquare = board.GetSquareAt(row + direction, col + offset);
                if (diagonalSquare != null && diagonalSquare.piece != null && diagonalSquare.piece.color != color)
                {
                    possibleMoves.Add(diagonalSquare);
                }
            }

            return possibleMoves;
        }
    }
}
