﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess
{
    public class Rook : Piece
    {
        public Rook(string color) : base(color, "Rook") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare, Board board)
        {
            var possibleMoves = new HashSet<Square>();

            // Straight directions: (rowOffset, colOffset)
            int[][] directions = { new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } };

            foreach (var dir in directions)
            {
                int row = currentSquare.row + dir[0];
                int col = currentSquare.col + dir[1];

                while (board.IsWithinBounds(row, col))
                {
                    var targetSquare = board.GetSquareAt(row, col);

                    if (targetSquare.piece != null)
                    {
                        if (targetSquare.piece.color != color) possibleMoves.Add(targetSquare); // Capture
                        break; // Stop moving further
                    }

                    possibleMoves.Add(targetSquare);
                    row += dir[0];
                    col += dir[1];
                }
            }

            return possibleMoves;
        }

    }
}