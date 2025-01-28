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
        public Square[,] grid;
        public string current_turn = "White"; //each player makes one move at a time, alternatively
        //for piece movement:
        public Square? StartSquare=null;
        public Square? EndSquare=null;
        public Square? ChessHere = null; //if a king is in chess his square will be highlighted
        public HashSet<Square> PossibleMoves { get; set; } = new(); //posible moves of potentially selected piece
        //MOVEHISTORY
        public List<Move> MoveHistory = new List<Move>();
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
        }

        public void ClearMovementData()
        {
            StartSquare = null;
            EndSquare = null;
            PossibleMoves.Clear();
        }

        public bool Try_Execute_Move()
        {
            if(StartSquare==null)
                return false;

            PossibleMoves = StartSquare.piece.GetPossibleMoves(StartSquare, this);
            if (Move_Is_Possible())
            {
                AddMoveToHistory(StartSquare.piece, StartSquare, EndSquare);
                MovePiece();
                ClearMovementData();
                if (GetKingPosition(current_turn).IsDangerous(this, current_turn))
                {
                    Console.WriteLine("your king is still in check dummy");
                    ReverseLastMove();
                    return false;
                }
                else
                {
                    SwitchTurn();
                    checkIfChessWasGiven();
                    return true;
                }
            }
            else
            {
                Console.WriteLine("no possible");
                EndSquare = null;
                return false;
            }
        }

        public bool ParameterMove(Square start, Square End) //The star of the show :D
        {
            StartSquare = start;
            EndSquare = End;
            return Try_Execute_Move();
        }

        public void ReverseLastMove()
        {
            if (MoveHistory.Count == 0)
            {
                Console.WriteLine("No moves to reverse.");
                return;
            }

            // Get the last move
            var lastMove = GetLastMove();

            // Restore the piece to the starting square
            StartSquare = lastMove.startpoint;
            EndSquare = lastMove.endpoint;

            // Move the piece back to the starting square
            StartSquare.piece = lastMove.piece;

            // If there was a captured piece, restore it to the endpoint
            EndSquare.piece = lastMove.capturedPiece; // Assuming `capturedPiece` tracks what was captured

            Console.WriteLine($"Restored piece to StartSquare: {StartSquare.piece?.type ?? "None"}");
            Console.WriteLine($"Restored piece to EndSquare: {EndSquare.piece?.type ?? "None"}");

            // Remove the last move from the history
            MoveHistory.RemoveAt(MoveHistory.Count - 1);
        }

        public void SwitchTurn()
        {
            if (current_turn == "White")
                current_turn = "Black";
            else
                current_turn = "White";
        }

        public bool Move_Is_Possible()
        {
             if (PossibleMoves.Contains(EndSquare))
                    return true;
                return false;
        }

        public void AddMoveToHistory(Piece piece, Square startpoint, Square endpoint)
        {
            MoveHistory.Add(new Move(piece, startpoint, endpoint, endpoint.piece));
        }

        public void PrintMoveHistory()
        {
            if (MoveHistory.Count == 0)
            {
                Console.WriteLine("No moves have been made yet.");
                return;
            }

            Console.WriteLine("Move History:");
            for (int i = 0; i < MoveHistory.Count; i++)
            {
                Console.WriteLine(String.Format("{0}. {1} {2} was moved to {3}.",
                i + 1,
                MoveHistory[i].piece.color,
                MoveHistory[i].piece.type,
                MoveHistory[i].endpoint.ToString()));
            }
        }
        public Move GetLastMove()
        {
            if (MoveHistory.Count >= 1)
                return MoveHistory[MoveHistory.Count - 1];
            else
                return null;
        }

        public Move GetLastMoveOfPlayer(string color)
        {
            for(int i=MoveHistory.Count-1; i>=0; i--)
            {
                if (MoveHistory[i].piece.color==color)
                {
                    return MoveHistory[i];
                }
            }
            return null;
        }

        public int GetNumberOfMoves(string color)
        {
            int nr = 0;
            for (int i = MoveHistory.Count - 1; i >= 0; i--)
            {
                if (MoveHistory[i].piece.color == color)
                {
                    nr++;
                }
            }
            return nr;
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
        public void checkIfChessWasGiven()
        {
            Move last = GetLastMove();
            if (last.piece.type == "King") //u cant give chess with a king
                ChessHere = null;
            else
            {
                if (last.endpoint.piece != null)
                {
                    HashSet<Square> ChessRange = last.endpoint.piece.GetChessRange(last.endpoint, this);
                    Square kingposition = GetKingPosition(current_turn);
                    if (kingposition != null)
                    {
                        if (ChessRange.Contains(kingposition))
                        {
                            Console.WriteLine(String.Format("Chess to {0} king.", current_turn));
                            ChessHere = kingposition;
                        }
                        else //no chess is given currently
                        {
                            ChessHere = null;
                        }
                    }
                    else
                        Console.WriteLine("Exception: king doesnt seem to be found on the board");
                }
            }
        }

        public bool Mate()
        {
            foreach (Square square in grid)
            {
                if (square == null || square.piece==null)
                    continue;
                if (square.piece.color == current_turn) //vezi daca cel la rand poate face vreo mutare care sa-l scoata din sah
                {
                    var possibilities = square.piece.GetPossibleMoves(square, this);
                    square.piece.PrintPossibleMoves(square, this);
                    foreach (Square destination in possibilities)
                    {
                        if (ParameterMove(square, destination))
                        {
                            ReverseLastMove();
                            SwitchTurn();
                            checkIfChessWasGiven();
                            return false;
                        }
                    }
                }
            }
            return true ; //there is no possible move to make. We have mare
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
                        if(square.piece.color == current_turn)
                        {
                            nr++;
                        }
                    }
                }
            }
            return nr;
        }

        public bool Draw()
        {
            if(GetKingPosition(current_turn).piece.GetPossibleMoves(GetKingPosition(current_turn),this)==null && GetNumberOfPieces(current_turn)==1)
            {
                return true;
            }
            return false;
        }

    }

}
