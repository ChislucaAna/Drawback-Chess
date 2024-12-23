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
                        if (!IsUnderAttack(board,targetSquare))
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

        public bool IsUnderAttack(Board board, Square targetsquare) //verifies if the current square would be dangerous(is under attack) for the king of specifed color
        {
            Console.WriteLine("CHECKING FOR SQUARE..");
            Console.WriteLine(targetsquare.row);
            Console.WriteLine(targetsquare.col);

            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    if (board.grid[row, col].piece != null) //foreach enemy piece we get chess range and verify if the square is there
                    {
                        if (board.grid[row, col].piece.color != this.color)
                        {
                            Console.WriteLine($"({row}, {col})");
                            var chessrange = board.grid[row, col].piece.GetChessRange(board.grid[row, col], board);
                            Console.WriteLine("Calculatinngg");
                            if (chessrange != null) //for the opposite king it would be null
                            {
                                Console.WriteLine("Chess range is");
                                foreach (var move in chessrange)
                                {
                                    Console.WriteLine($"({move.row}, {move.col})");
                                }
                                if (ContainsSquare(chessrange, targetsquare))
                                {
                                    Console.WriteLine("Found one");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool ContainsSquare(HashSet<Square> chessRange, Square targetSquare)
        {
            return chessRange.Any(square => square.row == targetSquare.row && square.col == targetSquare.col);
        }
    }
}
