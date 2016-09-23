using System.Threading.Tasks;

namespace BuildIt.Data.Sqlite.File
{
    public interface IFileService
    {
        Task<string> RetrieveNativePath(string filePath);
    }
}
