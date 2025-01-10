using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DrawbackChess.Classes
{
    public class Drawback
    {
        public string text; //UI TEXT
        public string type;
        public string parameter;

        public Dictionary<string, Func<Session,string, bool>> drawbackHandlers = new Dictionary< string, Func<Session, string, bool>>();



        public Drawback(string contents)
        {
            Random rnd = new Random();
            int index = rnd.Next(1, 75);
            string[] lines = contents.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines = lines.Select(line => line.TrimEnd('\r')).ToArray();
            for (int i = 0; i < lines.Length && index >= 0; i++)
            {
                index--;
                if (index == 0)
                {
                    string[] bucati = lines[i].Split(";");
                    this.text = bucati[0];
                    this.type = bucati[1];
                    this.parameter = bucati[2];
                }
            }
            Thread.Sleep(100);

            drawbackHandlers["location_not_allowed"] = location_not_allowed;
            drawbackHandlers["piece_not_allowed"] = piece_not_allowed;
            drawbackHandlers["limited_number_of_moves"] = limited_number_of_moves;
        }

        public bool location_not_allowed(Session current_session,string playercolor)
        {
            if (current_session.board.GetLastMoveOfPlayer(playercolor) != null)
                if (current_session.board.GetLastMoveOfPlayer(playercolor).endpoint.ToString() == this.parameter)
                    return true;
            return false;
        }

        public bool piece_not_allowed(Session current_session,string playercolor)
        {
            if (current_session.board.GetLastMoveOfPlayer(playercolor) != null)
                if (current_session.board.GetLastMoveOfPlayer(playercolor).piece.type == this.parameter)
                    return true;
            return false;
        }

        public bool limited_number_of_moves(Session current_session,string playercolor)
        {
            if (current_session.board.GetNumberOfMoves(playercolor) == Convert.ToInt32(this.parameter))
                return true;
            return false;
        }
    }
}
