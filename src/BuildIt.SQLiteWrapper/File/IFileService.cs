using System.Threading.Tasks;

namespace BuildIt.SQLiteWrapper.File
{
    public interface IFileService
    {
        Task<string> RetrieveNativePath(string filePath);
    }
}
