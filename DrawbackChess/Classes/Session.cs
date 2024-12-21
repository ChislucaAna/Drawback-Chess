using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    }
}
