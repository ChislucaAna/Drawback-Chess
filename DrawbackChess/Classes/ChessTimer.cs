using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Components.Pages;

namespace DrawbackChess.Classes
{
    public class ChessTimer
    {
        public Timer timer;
        public bool isPaused = true;
        private Action _refreshUI;
        public TimeSpan TimeLeft;
        string color;

        public ChessTimer(int minutes,string color, Action refreshUI)
        {
            _refreshUI = refreshUI;
            TimeLeft = TimeSpan.FromMinutes(minutes);
            this.color = color;
        }

        public void StartTimer()
        {
            if (timer == null) //Start a new Timer
            {
                timer = new Timer(UpdateTimer, null, 1000, 1000);
                isPaused = false;
            }
            else if (isPaused) //Unpause
            {
                isPaused = false;
            }
            _refreshUI.Invoke();
        }
        public void PauseTimer()
        {
            if (timer != null)
            {
                isPaused = true;
            }
            _refreshUI.Invoke();
        }
        public void EndTimer()
        {
            timer?.Dispose();
            timer = null;
            isPaused = false; ;
            TimeLeft = TimeSpan.Zero;
            _refreshUI.Invoke();
        }

        private void UpdateTimer(object? state)
        {
            if (TimeLeft > TimeSpan.Zero && !isPaused)
            {
                TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            }
            else if (TimeLeft <= TimeSpan.Zero)
            {
                EndTimer(); // Stop the timer when time is up
                //And award the win to other player, NO IDEA HOW TO GET THAT HERE
                //me neither
            }
            _refreshUI.Invoke();
        }
    }
}
