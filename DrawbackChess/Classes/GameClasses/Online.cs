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
        public string player2;
        public string drawback2;
        public string parameter2;
        public static string apiKey;
        public static AppConfig config;
        public Online()
        {

        }
        public class AppConfig
        {
            public AppSettings AppSettings { get; set; }
        }

        public class AppSettings
        {
            public string APIKey { get; set; }
        }

        public static class ConfigService
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

        public async Task OnlineAsync(string username, string drawback, string parameter)
        {
            // Read API Key
            string apiKey = config.AppSettings.APIKey.ToString();
            Console.WriteLine(apiKey);
            string id = Preferences.Get("app_unique_id", null);
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
                Preferences.Set("app_unique_id", id);
            }

            var settings = MongoClientSettings.FromConnectionString(apiKey);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            // Create a new client and connect to the server
            var client = new MongoClient(settings);

            Random rnd = new Random();

            var database = client.GetDatabase("chess_games");
            var matchmakeCollection = database.GetCollection<BsonDocument>("matchmaking");
            var boardCollection = database.GetCollection<BsonDocument>("boards");
            BsonDocument newGameBoard;

            //Search if you are already first player
            var filter = Builders<BsonDocument>.Filter.Eq("UID1", id);
            var firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

            //If you are first player
            if (firstDocument != null) 
            {
                var update = Builders<BsonDocument>.Update.Set("alive", "yes"); //Set global used everywhere

                //If the match you found is alive just get to it
                if (firstDocument.Contains("alive"))
                {
                    GameMenu.matchFound = true;
                    return;
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

                Console.WriteLine("Alive was set to true!");
                Console.WriteLine("Entered Match");
                GameMenu.matchFound = true;
                return;
            }

            filter = Builders<BsonDocument>.Filter.Eq("UID2", id);
            firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

            //If we are 2nd player
            if (firstDocument != null)
            {
                //Wait for alive
                while (!firstDocument.Contains("alive"))
                {
                    await Task.Delay(5000); // Non-blocking delay
                    firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();
                    Console.WriteLine("Looking for alive!");
                }

                Console.WriteLine("Alive found!");
                Console.WriteLine("Entered match!");
                GameMenu.matchFound = true;
                return;
            }

            filter = Builders<BsonDocument>.Filter.Eq("username2", BsonNull.Value);
            firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();

            //If you found somene else waiting for a match
            if (firstDocument != null)
            {
                Console.WriteLine("Someone is in the queue. Checking for alive...");

                //Get _id of the matchmaking request
                ObjectId insertedId = firstDocument["_id"].AsObjectId;
                filter = Builders<BsonDocument>.Filter.Eq("_id", insertedId);

                var update = Builders<BsonDocument>.Update
                    .Set("username2", username)
                    .Set("drawback2", drawback)
                    .Set("parameter2", parameter)
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

                Console.WriteLine("Alive found!");
                Console.WriteLine("Entered match!");
                GameMenu.matchFound = true;
                return;
            }

            //If you don't yet have a matchmaking request and there is no one else, make one
            if (firstDocument == null)
            {
                Console.WriteLine("No one in the queue. Entering queue...");

                var document = new BsonDocument
                {
                    { "username1", username },
                    { "drawback1", drawback },
                    { "parameter1", parameter },
                    { "UID1", id },
                    { "expireAfterSeconds", 10000 }
                };

                await matchmakeCollection.InsertOneAsync(document);

                filter = Builders<BsonDocument>.Filter.Eq("UID1", id);

                //If 2nd player is not present (AKA no ALIVE)
                while (!firstDocument.Contains("username2"))
                {
                    //Wait for 2nd player
                    await Task.Delay(5000); // Non-blocking delay
                    firstDocument = await matchmakeCollection.Find(filter).FirstOrDefaultAsync();
                    Console.WriteLine("Waiting for second player to join...");
                }

                Console.WriteLine("Second player joined!");
                string player2 = document["username2"].ToString();
                string drawback2 = document["drawback2"].ToString();
                string parameter2 = document["parameter2"].ToString();

                newGameBoard = new BsonDocument
                {
                    { "UID1", id },
                    { "UID2", firstDocument["UID2"] }
                };

                await boardCollection.InsertOneAsync(newGameBoard);

                var update = Builders<BsonDocument>.Update.Set("alive", "yes");
                await matchmakeCollection.UpdateOneAsync(filter, update);

                Console.WriteLine("Alive was set to true!");
                Console.WriteLine("Entered Match");
                GameMenu.matchFound = true;
                return;
            }
        }

        public static async Task<string> wait()
        {
            config = await ConfigService.LoadConfigAsync();
            Console.WriteLine(config.AppSettings.APIKey.ToString());
            return "uwu";
        }
    }
}
