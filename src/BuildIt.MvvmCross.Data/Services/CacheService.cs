using MvvmCross.Platform.Platform;
using MvvmCross.Plugins.File;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.MvvmCross.Data.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDictionary<string, object> dataCached = new Dictionary<string, object>();
        private readonly IDictionary<string, bool> dataRefreshed = new Dictionary<string, bool>();

        public bool KeyIsPath { get; set; }

        protected IMvxFileStore FileStore { get; set; }
        protected IMvxJsonConverter JsonConverter { get; set; }
        protected IDownloadService DownloadService { get; set; }

        public CacheService(IDownloadService downloadService, IMvxFileStore fileStore, IMvxJsonConverter jsonConverter)
        {
            FileStore = fileStore;
            JsonConverter = jsonConverter;
            DownloadService = downloadService;
        }

        private void EnsurePathExists(string localPath)
        {
            var path = Path.GetDirectoryName(localPath).Trim('\\');
            FileStore.EnsureFolderExists(path);
            return;
            //var dir = Path.GetDirectoryName(localPath).Replace("\\","/").Split('/');
            //var path = "/";
            //foreach (var dirPiece in dir)
            //{
            //    path = Path.Combine(path,dirPiece);
            //    FileStore.EnsureFolderExists(path);
            //}



        }


        private string KeyToFileName(string key)
        {
            if (KeyIsPath) return key;

            var sb = new StringBuilder();
            foreach (char c in key)
            {
                if (char.IsLetterOrDigit(c)) sb.Append(c);
            }
            return sb + ".json";
        }


#pragma warning disable 1998  // Async to allow for implementation
        public virtual async Task<TCached> LookupCachedValue<TCached>(string key)
#pragma warning restore 1998
        {
            try
            {
                object data;
                if (!dataCached.TryGetValue(key, out data))
                {
                    var fileName = KeyToFileName(key);

                    string json;
                    // Let's see if we have it on disk
                    if (FileStore.TryReadTextFile(fileName, out json))
                    {
                        return JsonConverter.DeserializeObject<TCached>(json);
                    }

                }
                return (TCached)data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default(TCached);
            }
        }

        public virtual async Task<TData> LoadCachedOrServiceData<TData>(string serviceUrl)
        {
            return await LoadCachedOrServiceData<object, TData>(serviceUrl, null);
        }

        public virtual async Task<TData> LoadCachedOrServiceData<TRequest, TData>(string serviceUrl, TRequest request)
            where TRequest : class
        {
            try
            {
                var data = await LookupCachedValue<TData>(serviceUrl);
                if (data != null)
                {
                    var updated = dataRefreshed.SafeValue(serviceUrl);
                    if (!updated)
                    {
                        dataRefreshed[serviceUrl] = true;
#pragma warning disable 4014 // Don't await this, just launch and forget
                        LoadServiceData<TRequest, TData>(serviceUrl, request);
#pragma warning restore 4014
                    }
                    return data;
                }

                data = await LoadServiceData<TRequest, TData>(serviceUrl, request);
                dataRefreshed[serviceUrl] = true;

                return data;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return default(TData);
            }
        }

        private async Task<TData> LoadServiceData<TRequest, TData>(string serviceUrl, TRequest request)
            where TRequest : class
        {
            try
            {
                var url = new Uri(serviceUrl);
                var data = await DownloadService.RequestAsync<TRequest, TData>(serviceUrl, request);

                await WriteDataToCache<TData>(serviceUrl, data);
                return data;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return default(TData);
            }
        }

        public virtual async Task<bool> WriteDataToCache<TData>(string serviceUrl, TData data)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var fileName = KeyToFileName(serviceUrl);

                    EnsurePathExists(fileName);

                    try
                    {
                        FileStore.DeleteFile(fileName);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    var json = JsonConverter.SerializeObject(data);
                    FileStore.WriteFile(fileName, json);

                    dataCached[serviceUrl] = data;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            });
        }
    }

    public interface ICacheService
    {
        bool KeyIsPath { get; set; }

        Task<TCached> LookupCachedValue<TCached>(string key);

        Task<TData> LoadCachedOrServiceData<TData>(string serviceUrl);

        Task<TData> LoadCachedOrServiceData<TRequest, TData>(string serviceUrl, TRequest request)
            where TRequest : class;

        Task<bool> WriteDataToCache<TData>(string serviceUrl, TData data);

    }



}