﻿using DrawbackChess.Classes.GameClasses;
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

        public async Task<int> SaveGameAsync(GameObject item)
        {
            await Init();
            if (item.Id != 0)
                return await Database.UpdateAsync(item);
            else
                return await Database.InsertAsync(item);
        }
    }
}