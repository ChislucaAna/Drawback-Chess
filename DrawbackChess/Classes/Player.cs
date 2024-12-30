using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace DrawbackChess.Classes
{
    public class Player
    {
        public string name;
        public TimeSpan TimeLeft;
        public Timer? timer;
        public bool isPaused = true;
        private Action? _refreshUI;
        public Drawback drawback;
        public Player(string name,Drawback drawback,TimeSpan initialtime, Action refreshUI)
        {
            this.name = name;
            this.TimeLeft = initialtime;
            this.drawback = drawback;   
            _refreshUI = refreshUI;
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
            isPaused = false;;
            TimeLeft = TimeSpan.Zero;
            _refreshUI.Invoke();
        }
        private async void UpdateTimer(object? state)
        {
            if (TimeLeft > TimeSpan.Zero && !isPaused)
            {
                TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
            }
            else if (TimeLeft <= TimeSpan.Zero)
            {
                EndTimer(); // Stop the timer when time is up
            }
            _refreshUI.Invoke();
        }
    }
}
