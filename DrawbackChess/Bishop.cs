using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess { 
    internal class Bishop :Piece
    {
        public Bishop(string color) : base(color, "Bishop") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare, Board board)
        {
            var possibleMoves = new HashSet<Square>();

            // Diagonal directions: (rowOffset, colOffset)
            int[][] directions = { new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { -1, -1 } };

            foreach (var dir in directions)
            {
                int row = currentSquare.row + dir[0];
                int col = currentSquare.col + dir[1];

                while (board.IsWithinBounds(row, col))
                {
                    var targetSquare = board.GetSquareAt(row, col);

                    if (targetSquare.piece != null)
                    {
                        if (targetSquare.piece.color != color) possibleMoves.Add(targetSquare); // Capture
                        break; // Stop moving further
                    }

                    possibleMoves.Add(targetSquare);
                    row += dir[0];
                    col += dir[1];
                }
            }

            return possibleMoves;
        }
    }
}
