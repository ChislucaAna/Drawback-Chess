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

namespace DrawbackChess
{
    public class Board
    {
        public static Square[,] grid;
        public static string current_turn = "White"; //each player makes one move at a time, alternatively
        public static Action refreshUI;
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
            SetupPieces();
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

        public static void PrintCurrentState()
        {
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    if (grid[row, col].piece != null)
                        Console.Write(grid[row, col].piece.type.ToString() + " ");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine(Environment.NewLine);
            }
        }

        public static Square? GetSquareAt(int row, int col)
        {
            return IsWithinBounds(row, col) ? grid[row, col] : null;
        }

        public static bool IsWithinBounds(int row, int col)
        {
            return row >= 1 && row <= 8 && col >= 1 && col <= 8;
        }


        public static void SwitchTurn()
        {
            if (current_turn == "White")
                current_turn = "Black";
            else
                current_turn = "White";
        }
        public static Square GetKingPosition(string color)
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
        public static  bool KingIsInCheck(string color)
        {
            
            Square kingposition = GetKingPosition(color);
            foreach (Square s in grid)
            {
                if (s == null || s.piece==null) continue;
                if (s.piece.color == color) continue;
                HashSet<Square> ChessRange = s.piece.GetChessRange(s);
                if (kingposition != null)
                {
                    if (ChessRange.Contains(kingposition))
                    {
                        Console.WriteLine(String.Format("Check to {0} king.", current_turn));
                        return true;
                        
                    }
                }
            }
            return false;
        }
        public static bool Mate()
        {
            foreach (Square square in grid)
            {
                if (square == null || square.piece==null)
                    continue;
                if (square.piece.color == current_turn) //vezi daca cel la rand poate face vreo mutare care sa-l scoata din sah
                {
                    HashSet<Square> possibilities = square.piece.GetPossibleMoves(square);
                    foreach (Square destination in possibilities)
                    {
                        if (MovementHandler.SimulateMove(square, destination)) //exista mutare care se poate face
                        {
                            return false;
                        }
                    }
                }
            }
            return true ;
        }
        public static int GetNumberOfPieces(string color)
        {
            int nr = 0;
            foreach (Square square in grid)
            {
                if (square != null)
                {
                    if (square.piece != null)
                    {
                        if(square.piece.color == current_turn)
                        {
                            nr++;
                        }
                    }
                }
            }
            return nr;
        }

        public static bool Draw()
        {
            if(GetKingPosition(current_turn).piece.GetPossibleMoves(GetKingPosition(current_turn))==null && GetNumberOfPieces(current_turn)==1)
            {
                return true;
            }
            return false;
        }

    }

}
