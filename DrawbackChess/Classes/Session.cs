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
        public static DrawbackHandler handler;
        public static ChessTimer WhiteTimer;
        public static ChessTimer BlackTimer;
        public static Action refreshUI;

        public Session()
        {

        }

        public static void RemoveOldGameData()
        {
            winner = null;
            typeofwin = null;
        }

        public static void SwitchTimer()
        {
            switch (board.current_turn)
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
            if (board.current_turn == "white")
                return player2;
            else
                return player1;
        }

        public static string GetTurnPlayerColor()
        {
            return board.current_turn;
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
                if (handler.handle[player.drawback.type](player.color, player.drawback.parameter))
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
            if (board.ChessHere != null) //There is a player currently in check. We check for mate.
            {
                if(board.Mate())
                {
                    typeofwin = "mate";
                    return GetLastThatMoved();
                }
                return null;
            }
            else //there is no player currently in check. We check for draws.
            {
                if(board.Draw())
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
            return board.MoveHistory != null;
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
