using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public class Board
    {
        public Square[,] grid;
        public Board()
        {
            grid = new Square[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    char columnLetter = (char)('A' + col);
                    grid[row, col] = new Square(columnLetter, row + 1);
                }
            }
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
    }
}
