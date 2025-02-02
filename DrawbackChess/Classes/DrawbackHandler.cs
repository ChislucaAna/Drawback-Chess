using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class DrawbackHandler
    {
        public static Dictionary<string, Func<string,string, bool>> handle =
            new Dictionary<string, Func<string,string, bool>>();

        public DrawbackHandler() 
        {
           
        }

        public static void InitDrawbacks()
        {
            handle["location_not_allowed"] = location_not_allowed;
            handle["piece_not_allowed"] = piece_not_allowed;
            handle["limited_number_of_moves"] = limited_number_of_moves;
        }

        public static bool location_not_allowed(string playercolor, string DrawbackParameter)
        {
            if (MoveHistory.GetLastMoveOfPlayer(playercolor) != null)
                if (MoveHistory.GetLastMoveOfPlayer(playercolor).endpoint.ToString() == DrawbackParameter)
                    return true;
            return false;
        }

        public static bool piece_not_allowed(string playercolor, string DrawbackParameter)
        {
            if (MoveHistory.GetLastMoveOfPlayer(playercolor) != null)
                if (MoveHistory.GetLastMoveOfPlayer(playercolor).piece.type == DrawbackParameter)
                    return true;
            return false;
        }

        public static bool limited_number_of_moves(string playercolor, string DrawbackParameter)
        {
            if (MoveHistory.GetNumberOfMoves(playercolor) == Convert.ToInt32(DrawbackParameter))
                return true;
            return false;
        }
    }
}
