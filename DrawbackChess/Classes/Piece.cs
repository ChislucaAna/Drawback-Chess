﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public abstract class Piece
    {
        public string color;
        public string type;
        public Piece(string color, string type)
        {
            this.color = color;
            this.type = type;
        }
        public abstract HashSet<Square> GetPossibleMoves(Square currentSquare, Board board);

        public void PrintPossibleMoves(Square currentSquare, Board board)
        {
            var possibleMoves = GetPossibleMoves(currentSquare, board);

            if (possibleMoves.Count == 0)
            {
                Console.WriteLine("No possible moves for this piece.");
                return;
            }

            Console.WriteLine($"Possible moves for the piece at ({currentSquare.row}, {currentSquare.col}):");
            foreach (var move in possibleMoves)
            {
                Console.WriteLine($"({move.row}, {move.col})");
            }
        }

    }
}