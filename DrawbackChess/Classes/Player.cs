using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DrawbackChess.Classes
{
    public class Player
    {
        public string name;
        public TimeSpan TimeLeft;
        public Timer timer;
        private bool isPaused = false;

        public Player(string name,TimeSpan initialtime)
        {
            this.name = name;
            this.TimeLeft = initialtime;
        }
        public void StartTimer()
        {
            if (timer == null) //Start a new Timer
            {
                timer = new Timer(UpdateTimer, null, 1000, 1000);
            }
            else if (isPaused) //Unpause
            {
                isPaused = false;
            }
        }
        public void PauseTimer()
        {
            if (timer != null)
            {
                isPaused = true;
            }
        }
        public void EndTimer()
        {
            timer?.Dispose();
            timer = null; 
            isPaused = false; 
        }
        private void UpdateTimer(object state)
        {
            if (!isPaused && TimeLeft > TimeSpan.Zero)
            {
                TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
                Console.WriteLine($"{name} Time Left: {TimeLeft.Minutes:D2}:{TimeLeft.Seconds:D2}");

                if (TimeLeft <= TimeSpan.Zero)
                {
                    EndTimer();
                    Console.WriteLine($"{name} has run out of time!");
                }
            }
        }
    }
}
