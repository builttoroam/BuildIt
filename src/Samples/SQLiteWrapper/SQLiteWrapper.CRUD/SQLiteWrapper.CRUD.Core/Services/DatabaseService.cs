using BuildIt.SQLiteWrapper.Database;
using BuildIt.SQLiteWrapper.Database.Interfaces;
using SQLite.Net;
using SQLiteWrapper.CRUD.Core.Models.Database;
using SQLiteWrapper.CRUD.Core.Services.Interfaces;

namespace SQLiteWrapper.CRUD.Core.Services
{
    public class DatabaseService : BasicDatabaseService, IDatabaseService
    {
        public DatabaseService(ISqlitePlatformProvider sqlitePlatformProvider, IDatabaseNameProvider databaseNameProvider, ILocalFileService localFileService) :
            base(sqlitePlatformProvider, databaseNameProvider, localFileService)
        {
        }

        protected override void CreateDatabaseTables(SQLiteConnection dbConnection)
        {
            dbConnection.CreateTable<PersonEntity>();
            dbConnection.CreateTable<AgencyEntity>();
        }
    }
}
