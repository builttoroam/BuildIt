using System;
using System.Threading.Tasks;
using Windows.Storage;
using SQLiteWrapper.CRUD.Core.Services.Interfaces;

namespace SQLiteWrapper.CRUD.UWP.Services
{
    public class LocalFileService : ILocalFileService
    {
        public async Task<string> RetrieveNativePath(string filePath)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
            return file.Path;
        }
    }
}
