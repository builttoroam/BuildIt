using BuildIt.Data.Sqlite.Database.Interfaces;

namespace BuildIt.Data.Sqlite.Database.Models.Results
{
    public class BaseSaveEntityResult : BaseResult, ISaveDataResult
    {
        public string NewEntityId { get; set; }
    }
}
