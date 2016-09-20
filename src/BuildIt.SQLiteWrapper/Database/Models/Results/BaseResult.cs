using BuildIt.Data.Sqlite.Database.Interfaces;

namespace BuildIt.Data.Sqlite.Database.Models.Results
{
    public class BaseResult : IDataResult
    {
        public bool Success { get; set; }
    }
}
