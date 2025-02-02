using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class MovementHandler
    {

        public static Square? StartSquare = null;
        public static Square? EndSquare = null;
        public static HashSet<Square> PossibleMoves { get; set; } = new();
        public MovementHandler()
        { 

        }

        public static void TrySelectPieceOn(Square s)
        {
            if (CanSelect(s))
            {
                StartSquare = s;
            }
        }

        public static bool CanSelect(Square s)
        {
            if (s.piece == null)
                return false; //there is no piece to select on this square
            if (Board.current_turn != s.get_piece_color())
                return false; //it s not this player's turn yet
            return true; //all is good
        }

        public static bool Move_Is_Possible()
        {
            if (PossibleMoves.Contains(EndSquare))
                return true;
            return false;
        }
        public static void DeselectPiece()
        {
            StartSquare = null;
            PossibleMoves.Clear();
        }

        static void MovePiece()
        {
            EndSquare.piece = StartSquare.piece;
            StartSquare.piece = null;
        }

        public static bool SimulateMove(Square start, Square end)
        {
            bool successful = true;
            StartSquare = start;
            EndSquare = end;

            MoveHistory.AddMoveToHistory(StartSquare.piece, StartSquare, EndSquare);
            MovePiece();
            ClearMovementData();
            if (Board.KingIsInCheck(Board.current_turn))
                successful = false;
            ReverseLastMove();
            MoveHistory.RemoveLastFromHistory();

            return successful;
        }

        public static void ReverseLastMove()
        {
            if (!MoveHistory.contents.Any())
            {
                Console.WriteLine("No moves to reverse.");
                return;
            }

            var lastMove = MoveHistory.GetLastMove();

            // Restore the piece to the starting square
            StartSquare = lastMove.startpoint;
            EndSquare = lastMove.endpoint;

            // Move the piece back to the starting square
            StartSquare.piece = lastMove.piece;

            // If there was a captured piece, restore it to the endpoint
            EndSquare.piece = lastMove.capturedPiece; // Assuming `capturedPiece` tracks what was captured

            Console.WriteLine("Last move has just been reversed from the board");
            Console.WriteLine(lastMove.ToString());
        }

        public static void ReverseLastMoveOfPlayer(string playercolor)
        {
            var lastMove = MoveHistory.GetLastMoveOfPlayer(playercolor);
            if (lastMove == null)
                return;
            // Restore the piece to the starting square
            StartSquare = lastMove.startpoint;
            EndSquare = lastMove.endpoint;

            // Move the piece back to the starting square
            StartSquare.piece = lastMove.piece;

            // If there was a captured piece, restore it to the endpoint
            EndSquare.piece = lastMove.capturedPiece; // Assuming `capturedPiece` tracks what was captured

            Console.WriteLine("Last move has just been reversed from the board");
            Console.WriteLine(lastMove.ToString());
        }

        public static void ClearMovementData()
        {
            StartSquare = null;
            EndSquare = null;
            PossibleMoves.Clear();
        }

        public static bool Try_Execute_Move()
        {
            if (StartSquare == null)
                return false;

            PossibleMoves = StartSquare.piece.GetPossibleMoves(StartSquare);
            if (Move_Is_Possible())
            {
                MoveHistory.AddMoveToHistory(StartSquare.piece, StartSquare, EndSquare);
                Console.WriteLine("MoveHistory:");
                Console.WriteLine(MoveHistory.PrintMoveHistory());
                MovePiece();
                ClearMovementData();
                if (Board.KingIsInCheck(Board.current_turn))
                {
                    ReverseLastMove();
                    return false;
                }
                else
                {
                    Board.SwitchTurn();
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

        public static bool ParameterMove(Square start, Square End) //The star of the show :D
        {
            StartSquare = start;
            EndSquare = End;
            return Try_Execute_Move();
        }
    }
}
