using System;
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

        public void sendMove (Board board)
        {
            var boardCollection = client.GetDatabase("chess_games").GetCollection<BsonDocument>("boards");

            var filter = Builders<BsonDocument>.Filter.Eq(playerNumber, id);
            var document = boardCollection.Find(filter).FirstOrDefault();

            var update = Builders<BsonDocument>.Update.Set("board", Board.ToFEN(board));
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

        public async void waitEnemyTurn ()
        {
            while (getMyColor() != getCurrentTurn())
            {
                await Task.Delay(5000);
            }
        }

        public string getMyColor ()
        {
            if (playerNumber == "UID1")
                return "White";
            else
                return "Black";
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
                    { "UID2", firstDocument["UID2"] }
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
                    { "currentTurn", "White" }
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
