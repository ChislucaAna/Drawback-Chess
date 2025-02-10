using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DrawbackChess.Classes;
using DrawbackChess;


public class DatabaseService //Singleton design pattern used here
{
    private static DatabaseService _instance;
    private SQLiteAsyncConnection _database;

    private DatabaseService()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "OldMatches.db");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Game>().Wait();
    }

    public static DatabaseService Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DatabaseService();
            return _instance;
        }
    }
    public Task<int> AddGameAsync(Game match) => _database.InsertAsync(match);
    public Task<List<Game>> GetGamesAsync() => _database.Table<Game>().ToListAsync();
}
