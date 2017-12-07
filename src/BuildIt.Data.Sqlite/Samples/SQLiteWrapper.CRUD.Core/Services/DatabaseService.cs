using BuildIt.Data.Sqlite.Database;
using BuildIt.Data.Sqlite.Database.Interfaces;
using SQLite;
using SQLiteWrapper.CRUD.Core.Models.Database;
using SQLiteWrapper.CRUD.Core.Services.Interfaces;

namespace SQLiteWrapper.CRUD.Core.Services
{
    public class DatabaseService : BasicDatabaseService, IDatabaseService
    {
        public DatabaseService(IDatabaseNameProvider databaseNameProvider, ILocalFileService localFileService) :
            base(databaseNameProvider, localFileService)
        {
        }

        protected override void CreateDatabaseTables(SQLiteConnection dbConnection)
        {
            dbConnection.CreateTable<PersonEntity>();
            dbConnection.CreateTable<AgencyEntity>();
        }
    }
}
