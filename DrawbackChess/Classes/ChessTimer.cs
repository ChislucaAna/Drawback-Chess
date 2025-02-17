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

        public ChessTimer(int minutes,string color)
        {
            TimeLeft = TimeSpan.FromMinutes(minutes);
            this.color = color;
        }

        public void StartTimer(Game game)
        {
            if (timer == null) //Start a new Timer
            {
                timer = new Timer(UpdateTimer,game, 1000, 1000);
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
                EndTimer(game);
                if (color == "White")
<<<<<<< HEAD
                    GamePage.currentGame.winner = GamePage.currentGame.player2;
                else
                    GamePage.currentGame.winner = GamePage.currentGame.player1;
                GamePage.currentGame.typeofwin = "time limit";
=======
                    game.winner = game.player2;
                else
                    game.winner = game.player1;
                game.typeofwin = "time limit";
>>>>>>> 0b28d9be5a8283241a830a6b3ac1aa4b14f5b755
            }
            game.refreshUI.Invoke();
        }
    }
}
