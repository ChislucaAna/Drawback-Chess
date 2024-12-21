using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Security.Cert;
using Xamarin.Google.Crypto.Tink.Signature;
using static Android.InputMethodServices.Keyboard;

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

                bool isFirstMove = (color == "White" && row == 2) || (color == "Black" && row == 7); //first move of the pawn is 2 squares
                if (isFirstMove)
                {
                    var twoSquareForward = board.GetSquareAt(row + 2 * direction, col);
                    if (twoSquareForward != null && twoSquareForward.piece == null)
                    {
                        possibleMoves.Add(twoSquareForward);
                    }
                }
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

        public HashSet<Square> GetDiagonalCapture(Square currentSquare,Board board)
        {
            var possibleMoves = new HashSet<Square>();

            int direction = (color == "White") ? 1 : -1; // White moves up, Black moves down
            int row = currentSquare.row;
            int col = currentSquare.col;
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
