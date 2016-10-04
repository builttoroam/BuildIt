using System;
using System.Diagnostics;
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

        private readonly TimeSpan defaultExpirationTime = new TimeSpan(0, 0, 10, 0);
        private DateTimeOffset expirationDate;

        public bool HasExpired { get; private set; } = true;
        public event EventHandler CacheExpired;

        public async Task<bool> Save(AppConfiguration appConfiguration, TimeSpan expirationTime = default(TimeSpan))
        {
            var res = false;

            if (appConfiguration == null) return res;

            try
            {
                var folder = await CreateFolderToSaveAsync();
                if (folder == null) return res;

                var file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                if (file == null) return res;

                var appConfigJson = JsonConvert.SerializeObject(appConfiguration);
                await file.WriteAllTextAsync(appConfigJson);

                SetCacheExpirationTime(expirationTime);

                res = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return res;
        }

        public async Task<AppConfiguration> LoadConfigData()
        {
            try
            {
                var folder = await CreateFolderToSaveAsync();
                if (folder == null) return null;

                var file = await folder.GetFileAsync(FileName);
                if (file == null) return null;

                var jsonData = await file.ReadAllTextAsync();
                if (string.IsNullOrEmpty(jsonData)) return null;

                return JsonConvert.DeserializeObject<AppConfiguration>(jsonData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }

        public async Task ClearData()
        {
            try
            {
                HasExpired = true;

                var folder = await CreateFolderToSaveAsync();
                if (folder == null) return;

                var file = await folder.GetFileAsync(FileName);
                if (file == null) return;

                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task<IFolder> CreateFolderToSaveAsync()
        {
            try
            {
                var rootFolder = FileSystem.LocalStorage;
                var folder = await rootFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }

        private void SetCacheExpirationTime(TimeSpan expirationTime)
        {
            expirationTime = expirationTime == default(TimeSpan) ? defaultExpirationTime : expirationTime;
            expirationDate = DateTimeOffset.UtcNow.Add(expirationTime);
            HasExpired = false;

            Task.Run(async () =>
            {
                var now = DateTimeOffset.UtcNow;
                while (now < expirationDate)
                {
                    await Task.Delay(10000); //MK check every 10s
                    now = DateTimeOffset.UtcNow;
                }
                await ClearData();
                CacheExpired?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
