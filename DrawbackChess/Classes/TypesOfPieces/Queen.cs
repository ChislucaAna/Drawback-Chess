using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Components.Pages;

namespace DrawbackChess
{
    internal class Queen:Piece
    {
        public Queen(string color) : base(color, "Queen") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare,Board board)
        {
            var possibleMoves = new HashSet<Square>();

            // Combine Rook and Bishop movement logic
            possibleMoves.UnionWith(GetLinearMoves(currentSquare, true, true,board)); // Horizontal and Vertical
            possibleMoves.UnionWith(GetLinearMoves(currentSquare, false, true,board)); // Diagonals

            return possibleMoves;
        }

        private HashSet<Square> GetLinearMoves(Square currentSquare, bool isStraight, bool isDiagonal,Board board)
        {
            var moves = new HashSet<Square>();
            int[][] directions = isStraight
                ? new[] { new[] { 0, 1 }, new[] { 0, -1 }, new[] { 1, 0 }, new[] { -1, 0 } } // Straight
                : new[] { new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { -1, -1 } }; // Diagonal

            foreach (var dir in directions)
            {
                int row = currentSquare.row + dir[0];
                int col = currentSquare.col + dir[1];

                while (Board.IsWithinBounds(row, col))
                {
                    var targetSquare = board.GetSquareAt(row, col);

                    if (targetSquare.piece != null)
                    {
                        if (targetSquare.piece.color != currentSquare.piece.color)
                        {
                            moves.Add(targetSquare); // Add capture move
                        }
                        break; // Stop at the first piece (cannot jump over)
                    }

                    moves.Add(targetSquare); // Add valid move
                    row += dir[0];
                    col += dir[1];
                }
            }

            return moves;
        }


        public override HashSet<Square> GetChessRange(Square currentSquare,Board board)
        {
            var possibleMoves = new HashSet<Square>();

            // Combine Rook and Bishop movement logic
            possibleMoves.UnionWith(GetLinearChess(currentSquare, true, true,board)); // Horizontal and Vertical
            possibleMoves.UnionWith(GetLinearChess(currentSquare, false, true,board)); // Diagonals

            return possibleMoves;
        }

        private HashSet<Square> GetLinearChess(Square currentSquare, bool isStraight, bool isDiagonal,Board board)
        {
            var moves = new HashSet<Square>();
            int[][] directions = isStraight
                ? new[] { new[] { 0, 1 }, new[] { 0, -1 }, new[] { 1, 0 }, new[] { -1, 0 } } // Straight
                : new[] { new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { -1, -1 } }; // Diagonal

            foreach (var dir in directions)
            {
                int row = currentSquare.row + dir[0];
                int col = currentSquare.col + dir[1];

                while (Board.IsWithinBounds(row, col))
                {
                    var targetSquare = board.GetSquareAt(row, col);
                    if (targetSquare != null)
                    {
                        if (targetSquare.piece != null)
                        {
                            moves.Add(targetSquare); // Add capture move
                            break; // Stop at the first piece
                        }

                        moves.Add(targetSquare); // Add valid move
                        row += dir[0];
                        col += dir[1];
                    }
                }
            }

            return moves;
        }
    }
}
