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

        public Player get_winner()
        {
            Player special_winner = get_special_winner();
            if (special_winner != null)
                return special_winner;
            Player basic_winner = get_basic_winner();
            if (basic_winner != null)
                return basic_winner;
            return null;
        }

        public Player get_special_winner()
        {
            if(player1.drawback.was_broken(this))
                return player1;
            if (player2.drawback.was_broken(this))
                return player2;
            return null;
        }

        public Player get_basic_winner() //PLACEHOLDER : NOT IMPLEMENTED YET
        {
            return null;
        }
    }
}
