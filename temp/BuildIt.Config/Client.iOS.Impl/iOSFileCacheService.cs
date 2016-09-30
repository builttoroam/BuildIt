using System;
using System.IO;
using System.Threading.Tasks;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;
using MvvmCross.Platform;
using PCLStorage;

namespace Client.iOS.Impl
{
    public class iOSFileCacheService : IFileCacheService
    {
        //private MvxFileSystem FileService => Mvx.Resolve
        private IFileSystem FileSystem { get; } = PCLStorage.FileSystem.Current;

        private const string FileName = "cachedJson.txt";

        public async Task<bool> Save(AppConfiguration appConfiguration)
        {
            var folderPath = GetLocalFolderPath();
            if (string.IsNullOrEmpty(folderPath)) return false;

            var fullPath = Path.Combine(folderPath, FileName);
            //Write the file
            //FileService.WriteFile(fullPath, JsonConvert.SerializeObject(appConfiguration));
            return true;
        }

        public async Task<AppConfiguration> LoadConfigData()
        {
            AppConfiguration cachedConfig = null;
            var folderPath = GetLocalFolderPath();
            if (string.IsNullOrEmpty(folderPath)) return cachedConfig;

            var fullPath = Path.Combine(folderPath, FileName);

            //if (!FileService.FolderExists(folderPath) || !FileService.Exists(fullPath))
            //{
                return cachedConfig;
            //}
            //Read the file
            //string appConfigJson;
            //if (FileService.TryReadTextFile(fullPath, out appConfigJson))
            //{
            //    cachedConfig = JsonConvert.DeserializeObject<AppConfiguration>(appConfigJson);
            //}
            //return cachedConfig;
        }

        public async Task ClearData()
        {
            var folderPath = GetLocalFolderPath();
            if (string.IsNullOrEmpty(folderPath)) return;

            var fullPath = Path.Combine(folderPath, FileName);
            //if (FileService.Exists(fullPath))
            {
            //    FileService.DeleteFile(fullPath);
            }
        }

        private string GetLocalFolderPath()
        {
            var folder = Environment.SpecialFolder.MyDocuments;
            var folderPath = Environment.GetFolderPath(folder);
            //FileService.EnsureFolderExists(folderPath);
            //if (FileService.FolderExists(folderPath))
            //{
                //return folderPath;
            //}
            return string.Empty;
        }
    }
}