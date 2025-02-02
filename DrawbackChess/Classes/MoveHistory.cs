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
            if(contents.Count == 0) return;
            contents.RemoveAt(contents.Count - 1);
            Console.WriteLine("Removed last move from histroy.This is the current state:");
            MoveHistory.PrintMoveHistory();
        }

        public static string PrintMoveHistory()//inca mai trebuie implementat pentru rocada si checkmate
        {
            string tostring="";
            if (contents.Count == 0)
            {
               return "No moves have been made yet.";
            }

            for (int i = 0; i < contents.Count; i++)
            {
                tostring += (i+1).ToString()+".";
                tostring += GetLastMove().ToString();
                tostring += System.Environment.NewLine;
            }
            return tostring;
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

        public static void RemoveLastMoveOfPlayer(string color)
        {
            for (int i = contents.Count - 1; i >= 0; i--)
            {
                if (contents[i].piece.color == color)
                {
                    contents.RemoveAt(i);
                }
            };
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
