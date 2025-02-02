using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class MoveHistory
    {
        public static List<Move> contents = new List<Move>();

        public static void AddMoveToHistory(Piece piece, Square startpoint, Square endpoint)
        {
            contents.Add(new Move(piece, startpoint, endpoint, endpoint.piece));
        }

        public static void RemoveLastFromHistory()
        {
            contents.RemoveAt(contents.Count - 1);
        }

        public static void PrintMoveHistory()
        {
            if (contents.Count == 0)
            {
                Console.WriteLine("No moves have been made yet.");
                return;
            }

            Console.WriteLine("Move History:");
            for (int i = 0; i < contents.Count; i++)
            {
                Console.WriteLine(String.Format("{0}. {1} {2} was moved to {3}.",
                i + 1,
                contents[i].piece.color,
                contents[i].piece.type,
                contents[i].endpoint.ToString()));
            }
        }
        public static Move GetLastMove()
        {
            if (contents.Count >= 1)
                return contents[contents.Count - 1];
            else
                return null;
        }

        public static Move GetLastMoveOfPlayer(string color)
        {
            for (int i = contents.Count - 1; i >= 0; i--)
            {
                if (contents[i].piece.color == color)
                {
                    return contents[i];
                }
            }
            return null;
        }

        public static int GetNumberOfMoves(string color)
        {
            int nr = 0;
            for (int i = MoveHistory.contents.Count - 1; i >= 0; i--)
            {
                if (MoveHistory.contents[i].piece.color == color)
                {
                    nr++;
                }
            }
            return nr;
        }
    }
}
