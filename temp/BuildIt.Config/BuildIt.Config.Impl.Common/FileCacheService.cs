using System;
using System.IO;
using System.Threading.Tasks;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;
using Newtonsoft.Json;
using PCLStorage;

namespace BuildIt.Config.Impl.Common
{
    public class FileCacheService : IFileCacheService
    {
        private IFileSystem FileSystem { get; } = PCLStorage.FileSystem.Current;

        private const string FolderName = "Cached";
        private const string FileName = "CachedJson.txt";

        public async Task<bool> Save(AppConfiguration appConfiguration)
        {
            if (appConfiguration == null) return false;

            var folder = await CreateFolderToSaveAsync();
            if (folder == null) return false;

            var file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            if (file == null) return false;

            var appConfigJson = JsonConvert.SerializeObject(appConfiguration);
            await file.WriteAllTextAsync(appConfigJson);
            return true;
        }

        public async Task<AppConfiguration> LoadConfigData()
        {
            var folder = await CreateFolderToSaveAsync();
            if (folder == null) return null;

            var file = await folder.GetFileAsync(FileName);
            if (file == null) return null;

            var jsonData = await file.ReadAllTextAsync();
            if (string.IsNullOrEmpty(jsonData)) return null;

            return JsonConvert.DeserializeObject<AppConfiguration>(jsonData);
        }

        public async Task ClearData()
        {
            var folder = await CreateFolderToSaveAsync();
            if (folder == null) return;

            var file = await folder.GetFileAsync(FileName);
            if (file == null) return;

            await file.DeleteAsync();
        }

        private async Task<IFolder> CreateFolderToSaveAsync()
        {
            var rootFolder = FileSystem.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
            return folder;
        }
    }
}
