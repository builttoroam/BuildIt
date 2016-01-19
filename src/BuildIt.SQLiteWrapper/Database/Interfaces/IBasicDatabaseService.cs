namespace BuiltToRoam.Data.SQLite.Database.Interfaces
{
    public interface IBasicDatabaseService : IBaseDatabaseService
    {
        void CloseDbConnection();
    }
}
