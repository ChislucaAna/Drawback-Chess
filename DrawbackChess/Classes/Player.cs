using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
        string color;
        public Player(string name,string color,Drawback drawback,TimeSpan initialtime, Action refreshUI)
        {
            this.color= color;  
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

        public bool broke_drawback(Session current_session)
        {
            Console.WriteLine(drawback.type);
            switch (drawback.type)
            {
                case "location_not_allowed":
                    if(current_session.board.GetLastMoveOfPlayer(this.color)!=null)
                        if (current_session.board.GetLastMoveOfPlayer(this.color).endpoint.ToString() == drawback.parameter)
                            return true;
                    break;
                case "piece_not_allowed":
                    if (current_session.board.GetLastMoveOfPlayer(this.color) != null)
                        if (current_session.board.GetLastMoveOfPlayer(this.color).piece.type == drawback.parameter)
                            return true;
                    break;
                case "limited_number_of_moves":
                        if (current_session.board.GetNumberOfMoves(this.color) == Convert.ToInt32(drawback.parameter))
                            return true;
                    break;
                default:
                    Console.WriteLine("Unknown color.");
                    break;
            }
            return false;
        }
    }
}
