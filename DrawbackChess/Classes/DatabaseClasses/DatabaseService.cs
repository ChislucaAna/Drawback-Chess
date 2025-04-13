using DrawbackChess.Classes.GameClasses;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using SQLite;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace DrawbackChess.Classes.DatabaseClasses
{
    public class DatabaseService
    {
        SQLiteAsyncConnection Database;

        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<GameObject>();
        }

        public async Task<List<GameObject>> GetGamesAsync()
        {
            await Init();
            return await Database.Table<GameObject>().ToListAsync();
        }

        public async void PrintAllGames()
        {
            await Init();
            var table = await Database.Table<GameObject>().ToListAsync();
            foreach (var game in table)
            {
                Console.WriteLine(game.ToString());    
            }
        }

        public async Task<List<SQLiteConnection.ColumnInfo>> GetGamesTableInfoAsync()
        {
            await Init();
            return await Database.GetTableInfoAsync("GameObject");
        }

        public void DeleteDatabaseAsync()
        { 

            if (File.Exists(Constants.DatabasePath))
            {
                File.Delete(Constants.DatabasePath);
                Console.WriteLine("Database was deleted.");
            }
            else
            {
                Console.WriteLine("Database was not found.");
            }
        }

        public async Task<int> SaveGameAsync(GameObject item)
        {
            await Init();
            Console.WriteLine("GameSaved");
            return await Database.InsertAsync(item);
        }

        public async Task<int> UpdateGameAsync(GameObject item)
        {
            await Init();
            Console.WriteLine("GameUpdated");
            return await Database.UpdateAsync(item);
        }

    }
}