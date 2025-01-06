using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class Drawback
    {
        public string text; //UI TEXT
        public string type;
        public string parameter;

        public Drawback(string contents)
        {
            Random rnd = new Random();
            int index = rnd.Next(1, 75);
            string[] lines = contents.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            lines = lines.Select(line => line.TrimEnd('\r')).ToArray();
            for (int i = 0; i < lines.Length && index >= 0; i++)
            {
                index--;
                if (index == 0)
                {
                    string[] bucati = lines[i].Split(";");
                    this.text = bucati[0];
                    this.type = bucati[1];
                    this.parameter = bucati[2];
                }
            }
            Thread.Sleep(100);
        }
    }
}
