using SQLite.Net.Interop;

namespace BuildIt.SQLiteWrapper.Database.Interfaces
{
    public interface ISqlitePlatformProvider
    {
        ISQLitePlatform SqLitePlatform { get; set; }
    }
}
