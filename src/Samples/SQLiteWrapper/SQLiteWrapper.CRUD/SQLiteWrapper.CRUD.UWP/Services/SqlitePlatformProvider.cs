using BuildIt.Data.Sqlite.Database.Interfaces;
using SQLite.Net.Interop;

namespace SQLiteWrapper.CRUD.UWP.Services
{
    public class SqlitePlatformProvider : ISqlitePlatformProvider
    {
        public ISQLitePlatform SqLitePlatform { get; set; }

        public SqlitePlatformProvider()
        {
            SqLitePlatform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        }
    }
}
