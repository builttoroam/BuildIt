namespace BuildIt.Data.Sqlite.Database.Interfaces
{
    public interface IBasicDatabaseService : IBaseDatabaseService
    {
        void CloseDbConnection();
    }
}
