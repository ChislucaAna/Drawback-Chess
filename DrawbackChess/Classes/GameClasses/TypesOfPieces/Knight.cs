﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes.GameClasses;
using DrawbackChess.Components.Pages;

namespace DrawbackChess
{
    internal class Knight:Piece
    {
        public Knight(string color) : base(color, "Knight") { }

        public override HashSet<Square> GetPossibleMoves(Square currentSquare,Board board)
        {
            var possibleMoves = new HashSet<Square>();

            // All possible moves for a knight
            int[][] offsets =
            {
            new[] { 2, 1 }, new[] { 2, -1 },
            new[] { -2, 1 }, new[] { -2, -1 },
            new[] { 1, 2 }, new[] { 1, -2 },
            new[] { -1, 2 }, new[] { -1, -2 }
            };

            foreach (var offset in offsets)
            {
                int row = currentSquare.row + offset[0];
                int col = currentSquare.col + offset[1];

                if (Board.IsWithinBounds(row, col))
                {
                    var targetSquare = board.GetSquareAt(row, col);

                    if (targetSquare.piece == null || targetSquare.piece.color != color)
                    {
                        possibleMoves.Add(targetSquare); // Empty square or capture
                    }
                }
            }

            return possibleMoves;
        }

        public override HashSet<Square> GetChessRange(Square currentSquare,Board board)
        {
            var chessrange = new HashSet<Square>();

            // All possible moves for a knight
            int[][] offsets =
            {
            new[] { 2, 1 }, new[] { 2, -1 },
            new[] { -2, 1 }, new[] { -2, -1 },
            new[] { 1, 2 }, new[] { 1, -2 },
            new[] { -1, 2 }, new[] { -1, -2 }
            };

            foreach (var offset in offsets)
            {
                int row = currentSquare.row + offset[0];
                int col = currentSquare.col + offset[1];

                if (Board.IsWithinBounds(row, col))
                {
                    var targetSquare = board.GetSquareAt(row, col);

                    if (targetSquare!=null)
                    {
                        chessrange.Add(targetSquare); // Empty square or capture
                    }
                }
            }

            return chessrange;
        }
    }
}
