﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using DrawbackChess.Components.Pages;
using MongoDB.Bson;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Security;
using DrawbackChess.Classes.DatabaseClasses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MongoDB.Driver.Search;


namespace DrawbackChess.Classes.GameClasses
{


    public class Online
    {
        public MongoClient client;
        public string id;

        public string player1;
        public string drawback1;
        public string parameter1;
        public string drawbackText1;

        public string player2;
        public string drawback2;
        public string parameter2;
        public string drawbackText2;

        public string currentTurn;
        public string playerNumber;

        private static string apiKey;
        private static AppConfig config;
        private Online()
        {
        }

        public Board getBoard ()
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            return Board.FromFEN(document["board"].ToString());
        }

        public async Task<string> updateTimer (Game game)
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            long timeWhite;
            long timeBlack;

            if (document.Contains("whiteTimeSet") && document.Contains("blackTimeSet"))
            {
                if (!document["whiteTimePaused"].ToBoolean())
                    timeWhite = document["whiteTimeLeft"].ToInt64() - (DateTimeOffset.Now.ToUnixTimeSeconds() - document["whiteTimeSet"].ToInt64());
                else
                    timeWhite = document["whiteTimeLeft"].ToInt64();

                if (!document["blackTimePaused"].ToBoolean())
                    timeBlack = document["blackTimeLeft"].ToInt64() - (DateTimeOffset.Now.ToUnixTimeSeconds() - document["blackTimeSet"].ToInt64());
                else
                    timeBlack = document["blackTimeLeft"].ToInt64();

                game.WhiteTimer = new ChessTimer((int)timeWhite, "White", true);
                game.BlackTimer = new ChessTimer((int)timeBlack, "Black", true);

                if (timeWhite <= 0)
                {
                    game.winner = game.player1;
                    await endGame();
                }

                if (timeBlack <= 0)
                {
                    game.winner = game.player2;
                    await endGame();
                }

                if (document["whiteTimePaused"].ToBoolean())
                {
                    game.WhiteTimer.PauseTimer(game);
                    game.BlackTimer.StartTimer(game);
                }
                else
                {
                    game.BlackTimer.PauseTimer(game);
                    game.WhiteTimer.StartTimer(game);
                }
            }
            return "updated";
        }

        public async Task<string> SaveRemote(GameObject game) //ai doar insert , playerul care da cleanup salveaza remote
        {
            var gameCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("PreviousGames");
                var document = new BsonDocument
                {
                    //uid1 si 2 pt selectie in lista in gamehistory
                    { "current_turn", game.current_turn },
                    { "typeofwin", game.typeofwin },
                    { "board", game.board }, 
                    //serialize player to do
                    { "player1", game.player1 },
                    { "player2", game.player2 },
                    { "winner", game.winner },
                    { "movehistory", game.MoveHistory },
                    { "timestamps", game.TimeStamps },
                };

                await gameCollection.InsertOneAsync(document);
                return "Game was saved to remote db";
        }
        public async Task<string> endGame()
        {

            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");
            var matchmakingCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("matchmaking");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            while (!document.Contains("alive"))
            {
                await Task.Delay(5000);
                document = boardCollection.Find(filter).FirstOrDefault();
            }

            boardCollection.DeleteMany(filter);
            matchmakingCollection.DeleteMany(filter);

            return "uwu";
        }

        public void declareWinner(string winner)
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            var update = Builders<BsonDocument>.Update.Set("winner", winner);
            boardCollection.UpdateOne(filter, update);
        }

        public void sendMove (Board board, ChessTimer timer, string color)
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            string enemyColor = (color == "white") ? ("black") : ("white");

            var update = Builders<BsonDocument>.Update.Set("board", Board.ToFEN(board)).
                Set(color + "TimeLeft", timer.TimeLeft.TotalSeconds).
                Set(color + "TimeSet", DateTimeOffset.Now.ToUnixTimeSeconds()).
                Set(color + "TimePaused", true).
                Set(enemyColor + "TimePaused", false);
            boardCollection.UpdateOne(filter, update);
        }

        public void switchTurn()
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");
            
            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();
            
            var update = Builders<BsonDocument>.Update.Set("currentTurn", document["currentTurn"] == "White" ? "Black" : "White");
            boardCollection.UpdateOne(filter, update);
        }

        public async Task<string> waitEnemyTurn ()
        {
            while (getMyColor() != getCurrentTurn())
            {
                await Task.Delay(5000);
            }

            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            if (document.Contains("winner"))
            {
                //Declara winner online
                var update = Builders<BsonDocument>.Update.Set("alive", "yes");
                boardCollection.UpdateOne(filter, update);
                return document["winner"].ToString();
            }

            return "none";
        }

        public string getMyColor ()
        {
            if (playerNumber == "UID1")
                return "White";
            else
                return "Black";
        }

        public string getMyName()
        {
            if (playerNumber == "UID1")
                return player1;
            else
                return player2;
        }

        public string getCurrentTurn ()
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();
            while (!document.Contains("currentTurn"))
            {
                document = boardCollection.Find(filter).FirstOrDefault();
                Thread.Sleep(5000);
            }
            return document["currentTurn"].ToString();
        }

        private class AppConfig
        {
            public AppSettings AppSettings { get; set; }
        }
        private class AppSettings
        {
            public string APIKey { get; set; }
        }
        private static class ConfigService
        {
            public static async Task<AppConfig> LoadConfigAsync()
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "DrawbackChess.appsettings.json"; // Replace with your actual namespace

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new FileNotFoundException($"Resource '{resourceName}' not found.");

                using StreamReader reader = new StreamReader(stream);
                string json = await reader.ReadToEndAsync();

                return JsonSerializer.Deserialize<AppConfig>(json);
            }
        }

        private static void setVariables(BsonDocument document, ref Online self)
        {
            self.player2 = document["username2"].ToString();
            self.drawback2 = document["drawback2"].ToString();
            self.parameter2 = document["parameter2"].ToString();
            self.drawbackText2 = document["drawbackText2"].ToString();

            self.player1 = document["username1"].ToString();
            self.drawback1 = document["drawback1"].ToString();
            self.parameter1 = document["parameter1"].ToString();
            self.drawbackText1 = document["drawbackText1"].ToString();

            Console.WriteLine("Alive was set to true!");
            Console.WriteLine("Entered Match");
            GameMenu.matchFound = true;
        }

        public static async Task<Online> Create(string username, string drawback, string parameter, string drawbackText)
        {
            // Read API Key
            Online self = new Online();
            string apiKey = config.AppSettings.APIKey.ToString();
            Console.WriteLine(apiKey);

            //Get unique ID per device to avoid making accounts
            string id = Preferences.Get("app_unique_id", null);
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
                Preferences.Set("app_unique_id", id);
            }
            self.id = id;
            var settings = MongoClientSettings.FromConnectionString(apiKey);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            // Create a new client and connect to the server
            self.client = new MongoClient(settings);

            var database = self.client.GetDatabase("chess_games");
            var matchmakeCollection = database.GetCollection<BsonDocument>("matchmaking");
            var boardCollection = database.GetCollection<BsonDocument>("boards");
            BsonDocument newGameBoard;

            //Search if you are already first player
            var filter = Builders<BsonDocument>.Filter.Eq("UID1", id);
            var firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

            //If you are first player
            if (firstDocument != null) 
            {
                self.playerNumber = "UID1";
                Console.WriteLine(self.playerNumber);
                var update = Builders<BsonDocument>.Update.Set("alive", "yes"); //Set global used everywhere

                //If the match you found is alive just get to it
                if (firstDocument.Contains("alive"))
                {
                    setVariables(firstDocument, ref self);
                    return self;
                }

                //If 2nd player is not present (no uername2), we wait for them
                while (!firstDocument.Contains("username2"))
                {
                    //Wait for 2nd player
                    await Task.Delay(5000); // Non-blocking delay
                    firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();
                    Console.WriteLine("Waiting for second player to join...");
                }

                //Get ID of 2nd player
                newGameBoard = new BsonDocument
                {
                    { "UID1", id },
                    { "UID2", firstDocument["UID2"] },
                    { "currentTurn", "White" },
                    { "board", "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr" }
                };

                await boardCollection.InsertOneAsync(newGameBoard);                
                await matchmakeCollection.UpdateOneAsync(filter, update);

                setVariables(firstDocument, ref self);
                return self;
            }

            filter = Builders<BsonDocument>.Filter.Eq("UID2", id);
            firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

            //If we are 2nd player
            if (firstDocument != null)
            {
                self.playerNumber = "UID2";
                Console.WriteLine(self.playerNumber);
                //Wait for alive
                while (!firstDocument.Contains("alive"))
                {
                    await Task.Delay(5000); // Non-blocking delay
                    firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();
                    Console.WriteLine("Looking for alive!");
                }

                setVariables(firstDocument, ref self);
                return self;
            }

            filter = Builders<BsonDocument>.Filter.Eq("username2", BsonNull.Value);
            firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

            //If you found somene else waiting for a match
            if (firstDocument != null)
            {
                self.playerNumber = "UID2";
                Console.WriteLine(self.playerNumber);
                Console.WriteLine("Someone is in the queue. Checking for alive...");

                //Get _id of the matchmaking request
                ObjectId insertedId = firstDocument["_id"].AsObjectId;
                filter = Builders<BsonDocument>.Filter.Eq("_id", insertedId);

                var update = Builders<BsonDocument>.Update
                    .Set("username2", username)
                    .Set("drawback2", drawback)
                    .Set("parameter2", parameter)
                    .Set("drawbackText2", drawbackText)
                    .Set("UID2", id);

                //Add yourself to the matchmaaking
                await matchmakeCollection.UpdateOneAsync(filter, update);

                firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

                //Wait for alive confirmation from player 1
                while (!firstDocument.Contains("alive"))
                {
                    await Task.Delay(5000); // Non-blocking delay
                    firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();
                    Console.WriteLine("Looking for alive!");
                }

                setVariables(firstDocument, ref self);
                return self;
            }

            //If you don't yet have a matchmaking request and there is no one else, make one
            if (firstDocument == null)
            {
                self.playerNumber = "UID1";
                Console.WriteLine(self.playerNumber);
                Console.WriteLine("No one in the queue. Entering queue...");

                var document = new BsonDocument
                {
                    { "username1", username },
                    { "drawback1", drawback },
                    { "parameter1", parameter },
                    { "drawbackText1", drawbackText },
                    { "UID1", id },
                    { "expireAfterSeconds", 10000 }
                };

                await matchmakeCollection.InsertOneAsync(document);

                filter = Builders<BsonDocument>.Filter.Eq("UID1", id);

                //If 2nd player is not present (AKA no ALIVE)
                while (!document.Contains("username2"))
                {
                    //Wait for 2nd player
                    await Task.Delay(5000); // Non-blocking delay
                    document = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();
                    Console.WriteLine("Waiting for second player to join...");
                }

                Console.WriteLine("Second player joined!");

                newGameBoard = new BsonDocument
                {
                    { "UID1", id },
                    { "UID2", document["UID2"] },
                    { "currentTurn", "White" },
                    { "board", "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr" },
                    { "whiteTimeLeft", 300 },
                    { "whiteTimePaused", true},
                    { "blackTimeLeft", 300 },
                    { "blackTimePaused", true},
                };

                await boardCollection.InsertOneAsync(newGameBoard);

                var update = Builders<BsonDocument>.Update.Set("alive", "yes");
                await matchmakeCollection.UpdateOneAsync(filter, update);

                setVariables(document, ref self);
                return self;
            }
            return null;
        }

        public static async Task<string> wait()
        {
            config = await ConfigService.LoadConfigAsync();
            Console.WriteLine(config.AppSettings.APIKey.ToString());
            return "uwu";
        }
    }
}
