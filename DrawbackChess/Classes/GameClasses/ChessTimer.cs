using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Components.Pages;

namespace DrawbackChess.Classes.GameClasses
{
    public class ChessTimer
    {
        public Timer timer;
        public bool isPaused = true;
        private Action _refreshUI;
        public TimeSpan TimeLeft;
        string color;

        public ChessTimer(int time, string color, bool fromSeconds = false)
        {
            if (fromSeconds)
                TimeLeft = TimeSpan.FromSeconds(time);
            else
                TimeLeft = TimeSpan.FromMinutes(time);
            this.color = color;
        }

        public void StartTimer(Game game)
        {
            if (timer == null) //Start a new Timer
            {
                timer = new Timer(UpdateTimer, game, 1000, 1000);
                isPaused = false;
            }
            else if (isPaused) //Unpause
            {
                isPaused = false;
            }
            game.refreshUI.Invoke();
        }
        public void PauseTimer(Game game)
        {
            if (timer != null)
            {
                isPaused = true;
            }
            game.refreshUI.Invoke();
        }
        public void EndTimer(Game game)
        {
            timer?.Dispose();
            timer = null;
            isPaused = false; ;
            TimeLeft = TimeSpan.Zero;
            game.refreshUI.Invoke();
        }

        private void UpdateTimer(object? state)
        {
            Game game = (Game)state!;
            if (TimeLeft > TimeSpan.Zero && !isPaused)
            {
                TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            }
            else if (TimeLeft <= TimeSpan.Zero)
            {
                game.EndGame();
                game.typeofwin = "time limit";
            }
            game.refreshUI.Invoke();
        }
    }
}
