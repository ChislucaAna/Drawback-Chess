using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

namespace DrawbackChess
{
    public class Session
    {
        public Board board;
        public Player player1;
        public Player player2;
        public Player winner;

        public Session(Player player1, Player player2)
        {
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

        public void LookForWinner()
        {
            Player winner = GetSpecialWinner() ?? GetBasicWinner();

            if (winner != null)
            {
                this.winner = winner;
                EndGame();
            }
        }

        public Player GetLastThatMoved()
        {
            if (board.current_turn == "white")
                return player1;
            else
                return player2;
        }
        public Player GetSpecialWinner()
        {
            if(player1.broke_drawback(this))
                return player1;
            if (player2.broke_drawback(this))
                return player2;
            return null; //no winner yet
        }

        public Player GetBasicWinner() //PLACEHOLDER : NOT IMPLEMENTED YET . Basic chess endgames function
        {
            return null;
        }

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
