using BuildIt.Data.Sqlite.Common;
using BuildIt.Data.Sqlite.Database.Interfaces;
using BuildIt.Data.Sqlite.File;
using SQLite;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
namespace BuildIt.Data.Sqlite.Database
{
    public abstract class BasicDatabaseService : BaseDatabaseService, IBasicDatabaseService
    {
        private readonly IDatabaseNameProvider databaseNameProvider;
        private readonly IFileService fileService;

        private readonly AutoResetEvent createSqlConnectionResetEvent = new AutoResetEvent(true);

        public SQLiteConnection SqliteDbConnection { get; private set; }

        protected BasicDatabaseService(IDatabaseNameProvider databaseNameProvider, IFileService fileService)
        {
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
            if (databaseNameProvider == null || fileService == null ) return null;

            try
            {
                return CreateSQLiteConnection(await fileService.RetrieveNativePath(databaseNameProvider.DatabaseName), creationCollisionOption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
        private SQLiteConnection CreateSQLiteConnection(string nativeDbPath, CreationCollisionOption creationCollisionOption)
        {
            if (string.IsNullOrEmpty(nativeDbPath) || databaseNameProvider == null || fileService == null ) return null;

            try
            {
                createSqlConnectionResetEvent.WaitOne();

                if (creationCollisionOption == CreationCollisionOption.OpenIfExists && SqliteDbConnection != null)
                {
                    return SqliteDbConnection;
                }

                SqliteDbConnection = new SQLiteConnection(nativeDbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
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
