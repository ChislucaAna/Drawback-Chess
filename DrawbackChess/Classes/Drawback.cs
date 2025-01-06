using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawbackChess.Classes
{
    public class Drawback
    {
        public string text; //UI TEXT
        public string type;
        public string parameter;

        public Drawback()
        {
            Random rnd = new Random();
            var path = Path.Combine(FileSystem.AppDataDirectory, "drawbacks.txt");
            using var reader = new StreamReader(path);
            int index = rnd.Next(1, 10);
            string line;
            while ((line = reader.ReadLine()) != null && index >= 0)
            {
                index--;
            }
            reader.Close();
            string[] bucati = line.Split(";");
            this.text = bucati[0];
            this.type = bucati[1];
            this.parameter = bucati[2];
            Thread.Sleep(100);
        }

    }
}
