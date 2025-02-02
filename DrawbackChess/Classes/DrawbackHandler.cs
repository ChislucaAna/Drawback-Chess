using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class DrawbackHandler
    {
        public Dictionary<string, Func<string,string, bool>> handle =
            new Dictionary<string, Func<string,string, bool>>();

        public DrawbackHandler() 
        {
            handle["location_not_allowed"] = location_not_allowed;
            handle["piece_not_allowed"] = piece_not_allowed;
            handle["limited_number_of_moves"] = limited_number_of_moves;
        }

        public bool location_not_allowed(string playercolor, string DrawbackParameter)
        {
            if (Session.board.GetLastMoveOfPlayer(playercolor) != null)
                if (Session.board.GetLastMoveOfPlayer(playercolor).endpoint.ToString() == DrawbackParameter)
                    return true;
            return false;
        }

        public bool piece_not_allowed(string playercolor, string DrawbackParameter)
        {
            if (Session.board.GetLastMoveOfPlayer(playercolor) != null)
                if (Session.board.GetLastMoveOfPlayer(playercolor).piece.type == DrawbackParameter)
                    return true;
            return false;
        }

        public bool limited_number_of_moves(string playercolor, string DrawbackParameter)
        {
            if (Session.board.GetNumberOfMoves(playercolor) == Convert.ToInt32(DrawbackParameter))
                return true;
            return false;
        }
    }
}
