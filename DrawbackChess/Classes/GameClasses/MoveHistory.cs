using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DrawbackChess.Classes.GameClasses
{
    public class MoveHistory
    {
        public List<Move> contents = new List<Move>();

        public void AddMoveToHistory(Piece piece, Square startpoint, Square endpoint)
        {
            contents.Add(new Move(piece, startpoint, endpoint, endpoint.piece));
        }

        public void RemoveLastFromHistory()
        {
            if (contents.Count == 0) return;
            contents.RemoveAt(contents.Count - 1);
        }

        public override string ToString()//inca mai trebuie implementat pentru rocada si checkmate
        {
            string tostring = "";
            if (contents.Count == 0)
            {
                return "No moves have been made yet.";
            }

            for (int i = 0; i < contents.Count; i++)
            {
                tostring += contents[i].ToString();
                tostring += ";";
            }
            tostring = tostring.Remove(tostring.Count() - 1);  
            return tostring;
        }

        /*public static MoveHistory CreateFromString(string s)
        {
            MoveHistory result = new MoveHistory();
            string[] mutari =s.Split(';');
            foreach(string mutare in mutari)
            {
                string[] patrate = mutare.Split('.');
                Square s1 = Square.FromString(patrate[0]);
                Square s2 = Square.FromString(patrate[1]);
                result.AddMoveToHistory(s1.piece, s1, s2);
            }
            return result;
        }*/
        public Move GetLastMove()
        {
            if (contents.Count >= 1)
                return contents[contents.Count - 1];
            else
                return null;
        }

        public Move GetLastMoveOfPlayer(string color)
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

        public void RemoveLastMoveOfPlayer(string color)
        {
            for (int i = contents.Count - 1; i >= 0; i--)
            {
                if (contents[i].piece.color == color)
                {
                    contents.RemoveAt(i);
                }
            };
        }

        public int GetNumberOfMoves(string color)
        {
            int nr = 0;
            for (int i = contents.Count - 1; i >= 0; i--)
            {
                if (contents[i].piece.color == color)
                {
                    nr++;
                }
            }
            return nr;
        }
    }
}
