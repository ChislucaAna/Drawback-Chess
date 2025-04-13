using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using static System.Collections.Specialized.BitVector32;
using SQLite;
using Newtonsoft.Json;
using DrawbackChess.Components.Pages;
using DrawbackChess.Classes.DatabaseClasses;
namespace DrawbackChess.Classes.GameClasses
{
    public class Game
    {
        public string current_turn { get; set; } = "White";
        public string? typeofwin { get; set; } = null;

        public Player winner;

        //These fields cannot be added into the db. They must be serialised first:
        public Board board { get; set; }
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public ChessTimer WhiteTimer { get; set; }
        public ChessTimer BlackTimer { get; set; }
        public Action refreshUI { get; set; }
        public MoveHistory moveHistory { get; set; }

        public string TimeStamps;

        public Game(Board board, Player player1, Player player2, ChessTimer whiteTimer, ChessTimer blackTimer)
        {
            this.board = board;
            this.player1 = player1;
            this.player2 = player2;
            WhiteTimer = whiteTimer;
            BlackTimer = blackTimer;
            moveHistory = new MoveHistory();
        }


        //Pt sqllite:
        public Game()
        {

        }

        public void SwitchTimer()
        {
            switch (current_turn)
            {
                case "White":
                    BlackTimer.PauseTimer(this);
                    WhiteTimer.StartTimer(this);
                    Console.WriteLine("White timer started");
                    break;
                case "Black":
                    WhiteTimer.PauseTimer(this);
                    BlackTimer.StartTimer(this);
                    Console.WriteLine("Black timer started");
                    break;
                default:
                    Console.WriteLine("Something went wrong when trying to switch timer");
                    break;
            }
        }

        public Player GetLastThatMoved()
        {
            if (current_turn == "white")
                return player2;
            else
                return player1;
        }

        //
        //Win checkker functions:
        //

        public string LookForWinner()
        {
            if(typeofwin=="timelimit")
                return current_turn;

            winner = GetSpecialWinner() ?? GetBasicWinner();
            if (winner == null) return null;
            return winner.color;
        }

        public bool GameHasEnded()
        {
            return typeofwin != null;
        }
        public Player GetSpecialWinner()
        {
            Console.WriteLine("Looking for special winner");
            foreach (var player in new[] { player1, player2 })
            {
                if (DrawbackHandler.handle[player.drawback.type](player.color, player.drawback.parameter, this))
                {
                    Console.WriteLine("broke drawback");
                    typeofwin = "drawback rules";
                    return player == player1 ? player2 : player1; //if one player broke the drawback, the other is the winner
                }
            }
            return null; // No winner yet
        }

        public Player GetBasicWinner()
        {
            if (board.KingIsInCheck(current_turn)) //Current player is in check. We check for mate.
            {
                if (Mate())
                {
                    typeofwin = "mate";
                    return GetLastThatMoved();
                }
                return null;
            }
            else //Current player is not in check. We check for draws.
            {
                if (Draw())
                {
                    typeofwin = "draw";
                    return player1; //we wont display this anyways,  but it shows that game has ended
                }
                return null;
            }
        }

        public bool Mate()
        {
            foreach (Square square in board.grid)
            {
                if (square == null || square.piece == null)
                    continue;
                if (square.piece.color == current_turn) //vezi daca cel la rand poate face vreo mutare care sa-l scoata din sah
                {
                    HashSet<Square> possibilities = square.piece.GetPossibleMoves(square, board);
                    foreach (Square destination in possibilities)
                    {
                        if (MovementHandler.SimulateMove(square, destination, this)) //exista mutare care se poate face
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool Draw()
        {
            Square kingPosition = board.GetKingPosition(current_turn);
            King king = kingPosition.piece as King;
            if (king.GetPossibleMoves(kingPosition, board) == null
                && board.GetNumberOfPieces(current_turn) == 1)
            {
                return true;
            }
            return false;
        }

        //
        //Game state functions:
        //

        public void SwitchTurn()
        {
            if (current_turn == "White")
                current_turn = "Black";
            else
                current_turn = "White";
        }
        public bool GameHasStarted()
        {
            return moveHistory.contents.Any();
        }

        public void EndGame()
        {
            WhiteTimer.EndTimer(this);
            BlackTimer.EndTimer(this);
            refreshUI();
        }

    }
}
