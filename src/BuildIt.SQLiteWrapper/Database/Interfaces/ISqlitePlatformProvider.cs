using SQLite.Net.Interop;

namespace BuiltToRoam.Data.SQLite.Database.Interfaces
{
    public interface ISqlitePlatformProvider
    {
        ISQLitePlatform SqLitePlatform { get; set; }
    }
}
