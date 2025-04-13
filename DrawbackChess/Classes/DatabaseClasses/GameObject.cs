using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace DrawbackChess.Classes.DatabaseClasses
{
    public class GameObject
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public string current_turn { get; set; } = "White"; //"White", "Black" or "None" if the game has ended
        public string? typeofwin { get; set; } = null;
        public string board { get; set; } //FEN
        public string player1 { get; set; } //CSV:name;color;drawback
        public string player2 { get; set; } //CSV:name;color;drawback
        public string? winner { get; set; } = null; //CSV:name;color;drawback

        public string MoveHistory; //CSV

        public string TimeStamps; //csv-dupa fiecare mutare salvezi cele 2 inturi de time remaining de la cei doi jucatori.

        public GameObject() { } //sqllite convention

        public GameObject(string current_turn, string? typeofwin, string board, string player1, string player2, string? winner, string MoveHistory,string TimeStamps)
        {
            this.current_turn = current_turn;
            this.typeofwin = typeofwin;
            this.board = board;
            this.player1 = player1;
            this.player2 = player2;
            this.winner = winner;
            this.MoveHistory = MoveHistory;
            this.TimeStamps = TimeStamps;
        }


        public override string ToString()//inca mai trebuie implementat pentru rocada si checkmate
        {
            return this.Id + ";" + this.current_turn;
        }
    }
}
