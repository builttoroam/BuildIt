using SQLite.Net.Interop;

namespace BuildIt.Data.Sqlite.Database.Interfaces
{
    public interface ISqlitePlatformProvider
    {
        ISQLitePlatform SqLitePlatform { get; set; }
    }
}
