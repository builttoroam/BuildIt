namespace BuildIt.SQLiteWrapper.Database.Interfaces
{
    public interface IBasicDatabaseService : IBaseDatabaseService
    {
        void CloseDbConnection();
    }
}
