﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public class Board
    {
        public Square[,] grid;
        public string current_turn = "White"; //each player makes one move at a time, alternatively
        //for piece movement:
        public Square? StartSquare=null;
        public Square? EndSquare=null;
        public HashSet<Square> PossibleMoves { get; set; } = new(); //posible moves of potentially selected piece
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

        public void PrintCurrentState()
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

        public Square? GetSquareAt(int row, int col)
        {
            return IsWithinBounds(row, col) ? grid[row, col] : null;
        }

        public bool IsWithinBounds(int row, int col)
        {
            return row >= 1 && row <= 8 && col >= 1 && col <= 8;
        }

        public bool CanSelect(Square s)
        {
            if (s.piece == null)
                return false; //there is no piece to select on this square
            if (current_turn != s.get_piece_color())
                return false; //it s not this player's turn yet
            return true; //all is good
        }
        public void TrySelectPieceOn(Square s)
        {
            if (CanSelect(s))
            {
                StartSquare = s;
                PossibleMoves = s.piece.GetPossibleMoves(s,this);
            }
        }
        public void DeselectPiece()
        {
            StartSquare = null;
            PossibleMoves.Clear();
        }

        public void MovePiece()
        {
            EndSquare.piece = StartSquare.piece;
            StartSquare.piece = null;

            //Finish Movement
            StartSquare = null;
            EndSquare = null;
            PossibleMoves.Clear();
            //Next Turrn
            if (current_turn == "White")
                current_turn = "Black";
            else
                current_turn = "White";
        }
    }

}