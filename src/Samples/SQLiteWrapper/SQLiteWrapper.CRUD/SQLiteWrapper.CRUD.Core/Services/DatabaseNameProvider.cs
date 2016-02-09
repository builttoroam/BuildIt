using BuildIt.SQLiteWrapper.Database.Interfaces;
using SQLiteWrapper.CRUD.Core.Common;

namespace SQLiteWrapper.CRUD.Core.Services
{
    public class DatabaseNameProvider : IDatabaseNameProvider
    {
        public string DatabseName { get; set; } = Constants.DatabaseName;
    }
}
