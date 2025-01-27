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
        public Board board;
        public Player player1;
        public Player player2;
        public Player winner;
        public string contents;
        public string typeofwin;
        DrawbackHandler handler;

        public Session(Player player1, Player player2)
        {
            handler = new DrawbackHandler();
            board = new Board();
            this.player1 = player1;
            this.player2 = player2;
        }

        public void SwitchTimer()
        {
            switch (board.current_turn)
            {
                case "White":
                    player2.PauseTimer();
                    player1.StartTimer();
                    Console.WriteLine("White timer started");
                    break;
                case "Black":
                    player1.PauseTimer();
                    player2.StartTimer();
                    Console.WriteLine("Black timer started");
                    break;
                default:
                    Console.WriteLine("Something went wrong when trying to switch timer");
                    break;
            }
        }

        public Player GetLastThatMoved()
        {
            if (board.current_turn == "white")
                return player2;
            else
                return player1;
        }

        public string GetTurnPlayerColor()
        {
            return board.current_turn;
        }

        //
        //Win checkker functions:
        //

        public void LookForWinner()
        {
            Player winner = GetSpecialWinner() ?? GetBasicWinner();

            if (winner != null)
            {
                this.winner = winner;
                EndGame();
            }
        }
        public Player GetSpecialWinner()
        {
            foreach (var player in new[] { player1, player2 })
            {
                if (handler.handle[player.drawback.type](this, player.color, player.drawback.parameter))
                {
                    typeofwin = "drawback rules";
                    return player == player1 ? player2 : player1; //if one player broke the drawback, the other is the winner
                }
            }
            return null; // No winner yet
        }

        public Player GetBasicWinner()
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
        public bool GameHasStarted()
        {
            return board.MoveHistory != null;
        }

        public bool GameHasEnded()
        {
            return winner!= null;
        }

        public void EndGame()
        {
            player1.EndTimer();
            player2.EndTimer();
        }
    }
}
