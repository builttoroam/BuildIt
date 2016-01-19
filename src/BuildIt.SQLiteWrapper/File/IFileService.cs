using System.Threading.Tasks;

namespace BuiltToRoam.Data.SQLite.File
{
    public interface IFileService
    {
        Task<string> RetrieveNativePath(string filePath);
    }
}
