using BuildIt.Data.Sqlite.Database.Interfaces;
using SQLiteWrapper.CRUD.Core.Common;

namespace SQLiteWrapper.CRUD.Core.Services
{
    public class DatabaseNameProvider : IDatabaseNameProvider
    {
        public string DatabaseName { get; set; } = Constants.DatabaseName;
    }
}
