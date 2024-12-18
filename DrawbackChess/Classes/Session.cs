using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using Microsoft.AspNetCore.Components;

namespace DrawbackChess
{
    public class Session
    {
        public Board board;
        public Player player1;
        public Player player2;

        public Session(string name1, string name2, TimeSpan time)
        {
            board = new Board();
            player1 = new Player(name1, time);
            player2 = new Player(name2, time);
        }
    }
}
