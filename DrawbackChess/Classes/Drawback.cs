using System;
using System.Collections.Generic;
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

        public Drawback()
        {
            //fixstupidbug();
            Random rnd = new Random();
            var path = Path.Combine(FileSystem.AppDataDirectory, "drawbacks.txt");
            //path = @"drawbacks.txt";
            Console.WriteLine(path);
            using var reader = new StreamReader(path);
            int index = rnd.Next(1, 7);
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

        public async void fixstupidbug()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DrawbackChess.Resources.Raw.drawbacks.txt";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);

            // Read the contents of the file
            string content = reader.ReadToEnd();

            // Write it to the app's data directory if needed
            var path = Path.Combine(FileSystem.AppDataDirectory, "drawbacks.txt");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, content);
            }
        }

    }
}
