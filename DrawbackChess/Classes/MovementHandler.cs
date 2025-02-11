using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Components.IndividualComponents;
using DrawbackChess.Components.Pages;

namespace DrawbackChess.Classes
{
    public class MovementHandler
    {
        public static HashSet<Square> PossibleMoves { get; set; } = new();
        public MovementHandler()
        { 

        }

        public static bool Move_Is_Possible(Square s) //According to basic chess movement rules
        {
            if (PossibleMoves.Contains(s))
                return true;
            return false;
        }

        static void MovePiece(Square StartSquare, Square EndSquare)
        {
            EndSquare.piece = StartSquare.piece;
            StartSquare.piece = null;
        }

        public static bool SimulateMove(Square StartSquare, Square EndSquare,Game game) //only use this for mate checking
        {
            bool successful = true;

            game.moveHistory.AddMoveToHistory(StartSquare.piece, StartSquare, EndSquare);
            MovePiece(StartSquare,EndSquare);
            PossibleMoves.Clear();
            if (game.board.KingIsInCheck(game.current_turn))
                successful = false;
            ReverseLastMove(game);
            game.moveHistory.RemoveLastFromHistory();

            return successful;
        }

        public static void ReverseLastMove(Game game)
        {
            if (!game.moveHistory.contents.Any())
            {
                Console.WriteLine("No moves to reverse.");
                return;
            }

            var lastMove = game.moveHistory.GetLastMove();

            // Restore the piece to the starting square
            Square StartSquare = lastMove.startpoint;
            Square EndSquare = lastMove.endpoint;

            // Move the piece back to the starting square
            StartSquare.piece = lastMove.piece;

            // If there was a captured piece, restore it to the endpoint
            EndSquare.piece = lastMove.capturedPiece; // Assuming `capturedPiece` tracks what was captured

            Console.WriteLine("Last move has just been reversed from the board");
            Console.WriteLine(lastMove.ToString());
        }

        public static void ReverseLastMoveOfPlayer(string playercolor,Game game)
        {
            var lastMove = game.moveHistory.GetLastMoveOfPlayer(playercolor);
            if (lastMove == null)
                return;
            // Restore the piece to the starting square
            Square StartSquare = lastMove.startpoint;
            Square EndSquare = lastMove.endpoint;

            // Move the piece back to the starting square
            StartSquare.piece = lastMove.piece;

            // If there was a captured piece, restore it to the endpoint
            EndSquare.piece = lastMove.capturedPiece; // Assuming `capturedPiece` tracks what was captured

            Console.WriteLine("Last move has just been reversed from the board");
            Console.WriteLine(lastMove.ToString());
        }


        public static bool Try_Execute_Move(Square StartSquare, Square EndSquare, Game game)
        {
            if (StartSquare == null)
                return false;

            PossibleMoves = StartSquare.piece.GetPossibleMoves(StartSquare,game.board);
            if (Move_Is_Possible(EndSquare))
            {
                game.moveHistory.AddMoveToHistory(StartSquare.piece, StartSquare, EndSquare);
                MovePiece(StartSquare, EndSquare);
                game.board.PrintCurrentState();

                Console.WriteLine(EndSquare.piece);
                PossibleMoves.Clear();
                if (game.board.KingIsInCheck(game.current_turn))
                {
                    ReverseLastMove(game);
                    return false;
                }
                else
                {
                    game.SwitchTurn();

                    //Save current state to db
                    game.Save();
                    game.LookForWinner();
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
    }
}
