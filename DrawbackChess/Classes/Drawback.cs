using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DrawbackChess.Classes
{
    public class Drawback
    {
        public string text; //UI TEXT
        public string type; //key for the function dictionary
        public string parameter; //parameter for the handler function

        public Drawback()
        {
            generateRandomDrawback();
        }

        private async void generateRandomDrawback() 
        {
            Random rnd = new Random();
            int index = rnd.Next(1, 75);

            string contents = await DrawbackHandler.GetDrawbackFileContents();
            string[] lines = contents
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.TrimEnd('\r'))
                .ToArray();

            if (index <= lines.Length)
            {
                string[] bucati = lines[index - 1].Split(';');
                this.text = bucati[0];
                this.type = bucati[1];
                this.parameter = bucati[2];
            }
        }

    }
}
