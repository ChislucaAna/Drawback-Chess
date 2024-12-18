using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    internal class King:Piece
    {
        public King(string color) : base(color, "King") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare, Board board)
        {
            var possibleMoves = new HashSet<Square>();

            int[] rowOffsets = { -1, 0, 1 };
            int[] colOffsets = { -1, 0, 1 };

            foreach (int rowOffset in rowOffsets)
            {
                foreach (int colOffset in colOffsets)
                {
                    if (rowOffset == 0 && colOffset == 0) continue; // Skip the current square

                    var targetSquare = board.GetSquareAt(currentSquare.row + rowOffset, currentSquare.col + colOffset);
                    if (targetSquare != null && (targetSquare.piece == null || targetSquare.piece.color != color))
                    {
                        possibleMoves.Add(targetSquare);
                    }
                }
            }

            return possibleMoves;
        }
    }
}
