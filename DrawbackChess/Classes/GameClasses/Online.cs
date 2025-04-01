using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json;
using System.Reflection.Metadata;
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
        public Online(string username, string drawback, string parameter)
        {
            // Read API Key
            string apiKey = config.AppSettings.APIKey.ToString();
           Console.WriteLine(apiKey);  
            var settings = MongoClientSettings.FromConnectionString(apiKey);

            //StreamReader sr = new StreamReader("APIKey.env");

            //var settings = MongoClientSettings.FromConnectionString(sr.ReadLine());

            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            var client = new MongoClient(settings);
            // Send a ping to confirm a successful connection

            Random rnd = new Random();
            int UID = rnd.Next(10000000, 99999999);


            var database = client.GetDatabase("chess_games");
            var matchmakeCollection = database.GetCollection<BsonDocument>("matchmaking");
            var boardCollection = database.GetCollection<BsonDocument>("boards");


            var firstDocument = matchmakeCollection.Find(new BsonDocument()).FirstOrDefault();
            if (firstDocument == null)
            {
                var document = new BsonDocument
                {
                    { "username1", username },
                    { "drawback1", drawback },
                    { "parameter1", parameter },
                    { "UID1", UID }
                };

                matchmakeCollection.InsertOne(document);

                ObjectId insertedId = document["_id"].AsObjectId;
                var filter = Builders<BsonDocument>.Filter.Eq("_id", insertedId);
                document = matchmakeCollection.Find(filter).FirstOrDefault();

                while (!document.Contains("username2"))
                {
                    Thread.Sleep(5000);
                    document = matchmakeCollection.Find(filter).FirstOrDefault();
                }

                player2 = document["username2"].ToString();
                drawback2 = document["drawback2"].ToString();
                parameter2 = document["parameter2"].ToString();

                var newGameBoard = new BsonDocument
                {
                    { "UID1", UID },
                    { "UID2", document["UID2"] }
                };

                boardCollection.InsertOne(newGameBoard);

                var update = Builders<BsonDocument>.Update.Set("alive", newGameBoard["_id"].AsObjectId.ToString());
                matchmakeCollection.UpdateOne(filter, update);

                Console.WriteLine("Everything OK!");
            }
            else
            {

                ObjectId insertedId = firstDocument["_id"].AsObjectId;
                var filter = Builders<BsonDocument>.Filter.Eq("_id", insertedId);

                var update = Builders<BsonDocument>.Update.Set("username2", username).Set("drawback2", drawback).Set("parameter2", parameter).Set("UID2", UID);
                matchmakeCollection.UpdateOne(filter, update);

                firstDocument = matchmakeCollection.Find(filter).FirstOrDefault();

                while (!firstDocument.Contains("alive"))
                {
                    Thread.Sleep(5000);
                    firstDocument = matchmakeCollection.Find(filter).FirstOrDefault();
                }

                Console.WriteLine("Everything OK!");
            }
        }

        public static async Task<string> wait()
        {
            config =await ConfigService.LoadConfigAsync();
            Console.WriteLine(config.AppSettings.APIKey.ToString());
            return "uwu";
        }
    }
}
