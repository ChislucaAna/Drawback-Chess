using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using static System.Collections.Specialized.BitVector32;
using SQLite;
using Newtonsoft.Json;
using DrawbackChess.Components.Pages;
namespace DrawbackChess
{
    public class Game
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string boardInFEN { get; set; }
        public string current_turn { get; set; } = "White";
        public string? typeofwin { get; set; } = null;

        //These fields cannot be added into the db. They must be serialised first
        [Ignore]public Board board { get; set; }
        [Ignore] public Player player1 { get; set; }
        [Ignore] public Player player2 { get; set; }
        [Ignore] public Player? winner { get; set; } = null;
        [Ignore] public ChessTimer WhiteTimer { get; set; }
        [Ignore] public ChessTimer BlackTimer { get; set; }
        [Ignore] public Action refreshUI { get; set; }

        [Ignore] public MoveHistory moveHistory { get; set; }
        public string Player1Json { get; set; }
        public string Player2Json { get; set; }
        public string WinnerJson { get; set; }
        public string WhiteTimerJson { get; set; }
        public string BlackTimerJson { get; set; }

        public Game(Board board, Player player1, Player player2, ChessTimer whiteTimer, ChessTimer blackTimer)
        {
            this.board = board;
            this.player1 = player1;
            this.player2 = player2;
            this.WhiteTimer = whiteTimer;
            this.BlackTimer = blackTimer;
            moveHistory = new MoveHistory();
        }


        //Pt sqllite:
        public Game() 
        {

        }

        public void Serialize()
        {
            boardInFEN = Board.ToFEN(board);
            Player1Json = JsonConvert.SerializeObject(player1);
            Player2Json = JsonConvert.SerializeObject(player2);
            WinnerJson = winner != null ? JsonConvert.SerializeObject(winner) : null;
            WhiteTimerJson = JsonConvert.SerializeObject(WhiteTimer);
            BlackTimerJson = JsonConvert.SerializeObject(BlackTimer);
        }

        public void Deserialize()
        {
            try
            {
                board = Board.FromFEN(boardInFEN); //Chestia asta cauzeaza eroarea
                player1 = JsonConvert.DeserializeObject<Player>(Player1Json);
                player2 = JsonConvert.DeserializeObject<Player>(Player2Json);
                winner = !string.IsNullOrEmpty(WinnerJson) ? JsonConvert.DeserializeObject<Player>(WinnerJson) : null;
                WhiteTimer = JsonConvert.DeserializeObject<ChessTimer>(WhiteTimerJson);
                BlackTimer = JsonConvert.DeserializeObject<ChessTimer>(BlackTimerJson);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.InnerException?.StackTrace}");
            }
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

        public void LookForWinner()
        {
            winner = GetSpecialWinner() ?? GetBasicWinner();

            if (winner != null)
                EndGame();
            else
                SwitchTimer();
        }
        public Player GetSpecialWinner()
        {
            Console.WriteLine("Looking for special winner");
            foreach (var player in new[] { player1, player2 })
            {
                if (DrawbackHandler.handle[player.drawback.type](player.color, player.drawback.parameter,this))
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
                if(Mate())
                {
                    typeofwin = "mate";
                    return GetLastThatMoved();
                }
                return null;
            }
            else //Current player is not in check. We check for draws.
            {
                if(Draw())
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
                        if (MovementHandler.SimulateMove(square, destination,this)) //exista mutare care se poate face
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

        public bool GameHasEnded()
        {
            return winner!= null;
        }

        public void EndGame()
        {
            WhiteTimer.EndTimer(this);
            BlackTimer.EndTimer(this);
            refreshUI();
        }

        public async void Save()
        {
            Serialize();
            await DatabaseService.Instance.AddGameAsync(this);
            Console.WriteLine("GameSaved");
        }
    }
}
