using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes.GameClasses
{
    public class Move
    {
        public Piece piece;
        public Square startpoint;
        public Square endpoint;
        public Piece capturedPiece;

        public static Dictionary<string, string> MoveAbbreviations = new Dictionary<string, string>
        {
            { "King", "K" },
            { "Queen", "Q" },
            { "Rook", "R" },
            { "Bishop", "B" },
            { "Knight", "N" },
            { "Pawn", "" } // Pawns have no abbreviation in movement notation
        };

        public Move(Piece piece, Square startpoint, Square endpoint, Piece capture)
        {
            this.piece = piece;
            this.endpoint = endpoint;
            this.startpoint = startpoint;
            capturedPiece = capture;
        }

        public bool IsCapture()
        {
            return capturedPiece != null;
        }

        public override string ToString()
        {
            return startpoint.ToString() + "." + endpoint.ToString();
        }
    }
}
