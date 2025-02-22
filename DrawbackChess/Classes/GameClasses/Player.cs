using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace DrawbackChess.Classes.GameClasses
{
    public class Player
    {

        //Player data
        public string name;
        public string color;
        public Drawback drawback;

        //Player's timer

        public Player(string name, string color, Drawback drawback)
        {
            this.color = color;
            this.name = name;
            this.drawback = drawback;
        }
    }
}
