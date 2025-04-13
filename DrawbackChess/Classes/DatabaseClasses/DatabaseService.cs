using DrawbackChess.Classes.GameClasses;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using SQLite;

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

            // Get the record with the highest ID
            var latestGame = await Database.Table<GameObject>()
                                           .OrderByDescending(g => g.Id)
                                           .FirstOrDefaultAsync();

            var maxId = latestGame?.Id ?? 0;

            if (item.Id > maxId)
            {
                return await Database.InsertAsync(item);
            }
            else
            {
                return await Database.UpdateAsync(item);
            }
        }

    }
}