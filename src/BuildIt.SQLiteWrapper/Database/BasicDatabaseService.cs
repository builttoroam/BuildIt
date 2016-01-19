using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuiltToRoam.Data.SQLite.Common;
using BuiltToRoam.Data.SQLite.Database.Interfaces;
using BuiltToRoam.Data.SQLite.Database.Models;
using BuiltToRoam.Data.SQLite.Database.Models.Results;
using BuiltToRoam.Data.SQLite.File;
using BuiltToRoam.Data.SQLite.Repository;
using SQLite.Net;
using SQLite.Net.Interop;
// ReSharper disable InconsistentNaming

namespace BuiltToRoam.Data.SQLite.Database
{
    public abstract class BasicDatabaseService : BaseDatabaseService, IBasicDatabaseService
    {
        private readonly ISqlitePlatformProvider sqlitePlatformProvider;
        private readonly IDatabaseNameProvider databaseNameProvider;
        private readonly IFileService fileService;

        private readonly AutoResetEvent createSqlConnectionResetEvent = new AutoResetEvent(true);

        public SQLiteConnection SqliteDbConnection { get; private set; }

        protected BasicDatabaseService(ISqlitePlatformProvider sqlitePlatformProvider, IDatabaseNameProvider databaseNameProvider, IFileService fileService)
        {
            this.sqlitePlatformProvider = sqlitePlatformProvider;
            this.databaseNameProvider = databaseNameProvider;
            this.fileService = fileService;
        }

        protected async override Task<SQLiteConnection> CreateSQLiteConnection()
        {
            var sqlConnection = await CreateSQLiteConnection(CreationCollisionOption.OpenIfExists);
            if (sqlConnection == null) return null;

            CreateDatabaseTables(sqlConnection);

            return sqlConnection;
        }

        public void CloseDbConnection()
        {
            if (SqliteDbConnection == null) return;

            SqliteDbConnection.Close();
            SqliteDbConnection.Dispose();
        }

        private async Task<SQLiteConnection> CreateSQLiteConnection(CreationCollisionOption creationCollisionOption)
        {
            if (databaseNameProvider == null || fileService == null || sqlitePlatformProvider == null) return null;

            try
            {
                return CreateSQLiteConnection(await fileService.RetrieveNativePath(databaseNameProvider.DatabseName), creationCollisionOption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
        private SQLiteConnection CreateSQLiteConnection(string nativeDbPath, CreationCollisionOption creationCollisionOption)
        {
            if (string.IsNullOrEmpty(nativeDbPath) || databaseNameProvider == null || fileService == null || sqlitePlatformProvider == null) return null;

            try
            {
                createSqlConnectionResetEvent.WaitOne();

                if (creationCollisionOption == CreationCollisionOption.OpenIfExists && SqliteDbConnection != null)
                {
                    return SqliteDbConnection;
                }

                SqliteDbConnection = new SQLiteConnection(sqlitePlatformProvider.SqLitePlatform, nativeDbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
                return SqliteDbConnection;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                createSqlConnectionResetEvent.Set();
            }

            return null;
        }
    }
}
