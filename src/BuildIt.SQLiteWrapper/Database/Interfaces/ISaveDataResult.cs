namespace BuildIt.Data.Sqlite.Database.Interfaces
{
    public interface ISaveDataResult:IDataResult
    {
        string NewEntityId { get; }
    }
}