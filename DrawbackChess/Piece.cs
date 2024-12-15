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
        public abstract HashSet<Square> GetPossibleMoves(Square currentSquare, Board board);
    }
}
