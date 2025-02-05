using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Components.Pages;

namespace DrawbackChess
{
    internal class King:Piece
    {
        public King(string color) : base(color, "King") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare)
        {
            var possibleMoves = new HashSet<Square>();

            int[] rowOffsets = { -1, 0, 1 };
            int[] colOffsets = { -1, 0, 1 };

            foreach (int rowOffset in rowOffsets)
            {
                foreach (int colOffset in colOffsets)
                {
                    if (rowOffset == 0 && colOffset == 0) continue; // Skip the current square

                    var targetSquare = GamePage.currentGame.board.GetSquareAt(currentSquare.row + rowOffset, currentSquare.col + colOffset);
                    if (targetSquare != null && (targetSquare.piece == null || targetSquare.piece.color != color))
                    {
                        if (!targetSquare.IsDangerousForKing(this.color))
                        {
                            possibleMoves.Add(targetSquare);
                        }
                    }
                }
            }

            Console.WriteLine("Possible moves");
            foreach (var move in possibleMoves)
            {
                Console.WriteLine($"({move.row}, {move.col})");
            }

            return possibleMoves;
        }

        //the other king must keep a 1 square distance at all times so we treat this rule as a pseudo chess
        public override HashSet<Square> GetChessRange(Square currentSquare)
        {
            var chessrange = new HashSet<Square>();
            int[] rowOffsets = { -1, 0, 1 };
            int[] colOffsets = { -1, 0, 1 };

            foreach (int rowOffset in rowOffsets)
            {
                foreach (int colOffset in colOffsets)
                {
                    if (rowOffset == 0 && colOffset == 0) continue; // Skip the current square

                    var targetSquare = GamePage.currentGame.board.GetSquareAt(currentSquare.row + rowOffset, currentSquare.col + colOffset);
                    if (targetSquare != null)
                    {
                         chessrange.Add(targetSquare);
                    }
                }
            }
            return chessrange;
        }
    }
}
