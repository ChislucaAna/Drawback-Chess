using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Components.Pages;

namespace DrawbackChess.Classes.GameClasses
{
    public class DrawbackHandler
    {
        public static Dictionary<string, Func<string, string, Game, bool>> handle =
            new Dictionary<string, Func<string, string, Game, bool>>();

        public DrawbackHandler()
        {

        }

        public static async Task<string> GetDrawbackContents()
        {
            return string.Join("\n", handle.Keys.ToArray());
        }
        public static void InitDrawbacks()
        {
            handle["location_not_allowed"] = location_not_allowed;
            handle["piece_not_allowed"] = piece_not_allowed;
            handle["limited_number_of_moves"] = limited_number_of_moves;
        }

        public static bool location_not_allowed(string playercolor, string DrawbackParameter, Game game)
        {
            if (game.moveHistory.GetLastMoveOfPlayer(playercolor) != null)
                if (game.moveHistory.GetLastMoveOfPlayer(playercolor).endpoint.ToString() == DrawbackParameter)
                    return true;
            return false;
        }

        public static bool piece_not_allowed(string playercolor, string DrawbackParameter, Game game)
        {
            if (game.moveHistory.GetLastMoveOfPlayer(playercolor) != null)
                if (game.moveHistory.GetLastMoveOfPlayer(playercolor).piece.type == DrawbackParameter)
                    return true;
            return false;
        }

        public static bool limited_number_of_moves(string playercolor, string DrawbackParameter, Game game)
        {
            if (game.moveHistory.GetNumberOfMoves(playercolor) == Convert.ToInt32(DrawbackParameter))
                return true;
            return false;
        }
    }
}
