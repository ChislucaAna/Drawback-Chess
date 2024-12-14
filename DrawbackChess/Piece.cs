using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public class Piece
    {
        public string color;
        public string type;
        public Piece(string color, string type)
        {
            this.color = color;
            this.type = type;
        }
    }
}
