using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using static System.Collections.Specialized.BitVector32;

namespace DrawbackChess
{
    public class Session
    {
        public static Board board;
        public static Player player1;
        public static Player player2;
        public static Player? winner;
        public static string? typeofwin;
        public static ChessTimer WhiteTimer;
        public static ChessTimer BlackTimer;
        public static Action refreshUI; //refreshes GamePage.razor but not the chessboard itself

        public Session()
        {

        }

        public static void RemoveOldGameData()
        {
            winner = null;
            typeofwin = null;
            Board.current_turn = "White";
        }

        public static void SwitchTimer()
        {
            switch (Board.current_turn)
            {
                case "White":
                    BlackTimer.PauseTimer();
                    WhiteTimer.StartTimer();
                    Console.WriteLine("White timer started");
                    break;
                case "Black":
                    WhiteTimer.PauseTimer();
                    BlackTimer.StartTimer();
                    Console.WriteLine("Black timer started");
                    break;
                default:
                    Console.WriteLine("Something went wrong when trying to switch timer");
                    break;
            }
        }

        public static Player GetLastThatMoved()
        {
            if (Board.current_turn == "white")
                return player2;
            else
                return player1;
        }

        public static string GetTurnPlayerColor()
        {
            return Board.current_turn;
        }

        //
        //Win checkker functions:
        //

        public static void LookForWinner()
        {
            winner = GetSpecialWinner() ?? GetBasicWinner();

            if (winner != null)
            {
                EndGame();
            }
        }
        public static Player GetSpecialWinner()
        {
            Console.WriteLine("this has been called");
            foreach (var player in new[] { player1, player2 })
            {
                if (DrawbackHandler.handle[player.drawback.type](player.color, player.drawback.parameter))
                {
                    Console.WriteLine("broke drawback");
                    typeofwin = "drawback rules";
                    return player == player1 ? player2 : player1; //if one player broke the drawback, the other is the winner
                }
            }
            return null; // No winner yet
        }

        public static Player GetBasicWinner()
        {
            if (Board.KingIsInCheck(Board.current_turn)) //Current player is in check. We check for mate.
            {
                if(Board.Mate())
                {
                    typeofwin = "mate";
                    return GetLastThatMoved();
                }
                return null;
            }
            else //Current player is not in check. We check for draws.
            {
                if(Board.Draw())
                {
                    typeofwin = "draw";
                    return player1; //we wont display this anyways,  but it shows that game has ended
                }
                return null;
            }
        }

        //
        //Game state functions:
        //
        public static bool GameHasStarted()
        {
            return MoveHistory.contents != null;
        }

        public static bool GameHasEnded()
        {
            return winner!= null;
        }

        public static void EndGame()
        {
            WhiteTimer.EndTimer();
            BlackTimer.EndTimer();
            refreshUI();
        }
    }
}
