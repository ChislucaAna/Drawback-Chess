using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Pipelines;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using DrawbackChess.Components.Pages;
using DrawbackChess.Classes.GameClasses;

namespace DrawbackChess
{
    public class Board
    {
        public static Dictionary<string, string> AbbToFEN = new Dictionary<string, string>
        {
            { "King", "K" },
            { "Queen", "Q" },
            { "Rook", "R" },
            { "Bishop", "B" },
            { "Knight", "N" },
            { "Pawn", "P" } // Pawns have abbreviations in fen
        };

        public static Dictionary<string, string> AbbFromFEN= new Dictionary<string, string>
        {
            { "K", "King" },
            { "Q", "Queen" },
            { "R", "Rook" },
            { "B", "Bishop" },
            { "N", "Knight" },
            { "P", "Pawn" } // Pawns have abbreviations in fen
        };

        //vreau sa pot creea mai multe instante de tabla
        public Square[,] grid;
        //dar doar un meci ruleaza simultan
        public Action refreshUI;
        public Board()
        {
            grid = new Square[9, 9];
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    grid[row, col] = new Square(col, row);
                }
            }
            //SetupPieces();
        }

        public void SetupPieces()
        {
            // Place white pieces
            grid[1, 1].piece = new Rook("White");
            grid[1, 2].piece = new Knight("White");
            grid[1, 3].piece = new Bishop("White");
            grid[1, 4].piece = new Queen("White");
            grid[1, 5].piece = new King("White");
            grid[1, 6].piece = new Bishop("White");
            grid[1, 7].piece = new Knight("White");
            grid[1, 8].piece = new Rook("White");
            for (int col = 1; col <= 8; col++)
            {
                grid[2, col].piece = new Pawn("White");
            }

            // Place black pieces
            grid[8, 1].piece = new Rook("Black");
            grid[8, 2].piece = new Knight("Black");
            grid[8, 3].piece = new Bishop("Black");
            grid[8, 4].piece = new Queen("Black");
            grid[8, 5].piece = new King("Black");
            grid[8, 6].piece = new Bishop("Black");
            grid[8, 7].piece = new Knight("Black");
            grid[8, 8].piece = new Rook("Black");
            for (int col = 1; col <= 8; col++)
            {
                grid[7, col].piece = new Pawn("Black");
            }
        }

        public void PrintCurrentState()
        {
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    if (grid[row, col].piece != null)
                        Console.Write(grid[row, col].piece.type.ToString() + " ");
                    else
                        Console.Write("e ");
                }
                Console.WriteLine(Environment.NewLine);
            }
        }

        public Square? GetSquareAt(int row, int col)
        {
            return IsWithinBounds(row, col) ? grid[row, col] : null;
        }

        public static bool IsWithinBounds(int row, int col)
        {
            return row >= 1 && row <= 8 && col >= 1 && col <= 8;
        }


        public Square GetKingPosition(string color)
        {
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    if (grid[row,col].piece!=null)
                        if (grid[row, col].piece.type=="King" && grid[row, col].piece.color == color)
                            return grid[row, col];
                }
            }
            return null;
        }
        public bool KingIsInCheck(string color)
        {

            Square kingposition = GetKingPosition(color);
            foreach (Square s in grid)
            {
                if (s == null || s.piece == null) continue;
                if (s.piece.color == color) continue;
                HashSet<Square> ChessRange = s.piece.GetChessRange(s, this);
                if (kingposition != null)
                {
                    if (ChessRange.Contains(kingposition))
                    {
                        Console.WriteLine(String.Format("Check to {0} king.", color));
                        return true;

                    }
                }
            }
            return false;
        }

        public int GetNumberOfPieces(string color)
        {
            int nr = 0;
            foreach (Square square in grid)
            {
                if (square != null)
                {
                    if (square.piece != null)
                    {
                        if (square.piece.color == color)
                        {
                            nr++;
                        }
                    }
                }
            }
            return nr;
        }

        public static string ToFEN(Board b) //vreau sa mearga si pt alte forme/dimensiuni de tabla(nu neaparat 8*8)
        {
            string result = "";
            int empty_squares = 0;
            foreach (Square s in b.grid)
            {
                if (s==null || !IsWithinBounds(s.row, s.col)) continue;
                if (s.piece == null)//square is empty, increment
                    empty_squares++;
                else//square is not empty,write piece
                {
                    if (empty_squares != 0)//write how many aempty squares were before this piece
                    {
                        result += empty_squares.ToString();
                        empty_squares = 0;
                    }
                    if (s.piece.color == "Black")
                        result += AbbToFEN[s.piece.type].ToLower();
                    else
                        result += AbbToFEN[s.piece.type];
                }
                if (s.col == 8) //end of line,write any empty squares
                {
                    if (empty_squares != 0)
                    {
                        result += empty_squares.ToString();
                        empty_squares = 0;
                    }
                }
                if(s.col==8 && s.row != 8)//end of line,write separator
                    result += "/";
            }
            Console.WriteLine("ToFEN");
            Console.WriteLine(result);
            return result;
        }

        public static Board FromFEN(string fen)
        {
            Board result = new Board();
            string[] rows = fen.Split('/');
            int i = 1;
            int j = 1;
            foreach (string row in rows)
            {
                foreach (char c in row)
                {
                    if (Char.IsLetter(c))
                    {
                        string piece_type = AbbFromFEN[c.ToString().ToUpper()];
                        if (Char.IsLower(c)) //black piece
                        {
                            result.grid[i, j].piece = Piece.CreatePiece("Black", piece_type);
                        }
                        else //white piece
                        {
                            result.grid[i, j].piece = Piece.CreatePiece("White", piece_type);
                        }
                        j++;
                    }
                    else
                    {
                        j += Convert.ToInt32(c.ToString());
                    }
                }
                i++;
                j = 1;
            }
            Console.WriteLine("FromFEN");
            result.PrintCurrentState();
            return result;
        }

    }

}
