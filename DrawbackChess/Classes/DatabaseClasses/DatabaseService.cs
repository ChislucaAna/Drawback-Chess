using DrawbackChess.Classes.GameClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace DrawbackChess.Classes.DatabaseClasses
{
    public class DatabaseService : DbContext
    {
        public DbSet<Test> Tests { get; set; }
        public string DbPath { get; }

        public DatabaseService()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "app.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}