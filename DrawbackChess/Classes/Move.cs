using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class Move
    {
        public Piece piece;
        public Square startpoint;
        public Square endpoint;

        public Move(Piece piece, Square startpoint,Square endpoint)
        {
            this.piece = piece; 
            this.endpoint = endpoint;
            this.startpoint = startpoint;
        }
    }
}
