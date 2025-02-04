using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public abstract class Piece
    {
        public string color;
        public string type;
        public Piece(string color, string type)
        {
            this.color = color;
            this.type = type;
        }
        public abstract HashSet<Square> GetPossibleMoves(Square currentSquare);

        public abstract HashSet<Square> GetChessRange(Square currentSquare);
        public void PrintPossibleMoves(Square currentSquare, Board board)
        {
            var possibleMoves = GetPossibleMoves(currentSquare);

            if (possibleMoves.Count == 0)
            {
                Console.WriteLine("No possible moves for this piece.");
                return;
            }

            Console.WriteLine($"Possible moves for the piece at ({currentSquare.row}, {currentSquare.col}):");
            foreach (var move in possibleMoves)
            {
                Console.WriteLine($"({move.row}, {move.col})");
            }
        }
        public static Piece CreatePiece(string color, string pieceType)
        {
            switch (pieceType.ToLower())
            {
                case "pawn":
                    return new Pawn(color);
                case "knight":
                    return new Knight(color);
                case "bishop":
                    return new Bishop(color);
                case "rook":
                    return new Rook(color);
                case "queen":
                    return new Queen(color);
                case "king":
                    return new King(color);
                default:
                    throw new ArgumentException("Invalid piece type: " + pieceType);
            }
        }

    }
}
