using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading;
using Android.Widget;

namespace DrawbackChess.Classes.GameClasses
{
    public class Online
    {
        public MongoClient client;
        public string player2;
        public string drawback2;
        public string parameter2;
        public Online (string username, string drawback, string parameter)
        {
            const string connectionUri = "mongodb+srv://rapsyjigo:oybmFIyCs7XxxVpV@cluster0.el5cb.mongodb.net/?appName=Cluster0";
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            client = new MongoClient(settings);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var database = client.GetDatabase("chess_games");
            var collection = database.GetCollection<BsonDocument>("matchmaking");


            var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();
            if (firstDocument == null)
            {
                var document = new BsonDocument
                {
                    { "username1", username },
                    { "drawback1", drawback },
                    { "parameter1", parameter }
                };

                collection.InsertOne(document);

                ObjectId insertedId = document["_id"].AsObjectId;
                var filter = Builders<BsonDocument>.Filter.Eq("_id", insertedId);
                document = collection.Find(filter).FirstOrDefault();

                while (document["username2"] == null)
                {
                    Thread.Sleep(5000);
                    document = collection.Find(filter).FirstOrDefault();
                }

                player2 = document["username2"].ToString();
                drawback2 = document["drawback2"].ToString();
                parameter2 = document["parameter2"].ToString();

                var update = Builders<BsonDocument>.Update.Set("alive", username);
                collection.UpdateOne(filter, update);

                Console.WriteLine("Everything OK!");
            }
            else
            {

                ObjectId insertedId = firstDocument["_id"].AsObjectId;
                var filter = Builders<BsonDocument>.Filter.Eq("_id", insertedId);

                var update = Builders<BsonDocument>.Update.Set("username2", username).Set("drawback2", drawback).Set("parameter2", parameter);
                collection.UpdateOne(filter, update);

                firstDocument = collection.Find(filter).FirstOrDefault();

                while (firstDocument["alive"] == null)
                {
                    Thread.Sleep(5000);
                    firstDocument = collection.Find(filter).FirstOrDefault();
                }

                Console.WriteLine("Everything OK!");
            }
        }
    }
}
