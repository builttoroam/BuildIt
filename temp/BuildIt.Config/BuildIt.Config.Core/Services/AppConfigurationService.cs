using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BuildIt.Config.Core.Api.Models;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;
using MvvmCross.Platform;
using Newtonsoft.Json;

namespace BuildIt.Config.Core.Services
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly IAppConfigurationServiceEndpoint serviceEndpoint;

        public IVersionService VersionService { get; }

        private readonly AutoResetEvent getAppConfigurationAutoResetEvent = new AutoResetEvent(true);

        private bool isInitialized;

        private string currentAppConfigurationMd5Hash;

        private IUserDialogs dialogs;
        private IUserDialogs Dialogs
        {
            get
            {
                return dialogs ?? (dialogs = Mvx.Resolve<IUserDialogs>());
            }
            set
            {
                dialogs = value;
            }
        }

        private CancellationTokenSource cancellationToken;

        public AppConfigurationMapper Mapper { get; } = new AppConfigurationMapper();
        public AppConfiguration AppConfig { get; private set; }

        public AppConfigurationService(IAppConfigurationServiceEndpoint serviceEndpoint, IVersionService versionService)
        {
            this.serviceEndpoint = serviceEndpoint;
            this.VersionService = versionService;
        }

        public async Task<AppConfiguration> LoadAppConfig(bool retrieveCachedVersion = true)
        {
            if (AppConfig != null) return AppConfig;
            return await RetrieveAppConfig(retrieveCachedVersion);
        }


        private async Task<AppConfiguration> RetrieveAppConfig(bool retrieveCachedVersion = true)
        {
            //TODO: Handle this situation in a better way
            if (!isInitialized) return null;

            cancellationToken?.Cancel();

            return await Task.Run(async () =>
            {
                if (retrieveCachedVersion && AppConfig != null) return AppConfig;

                try
                {
                    if (getAppConfigurationAutoResetEvent.WaitOne())
                    {
                        //MK After waiting for other thread to finish retrieving the app config, check if we can return cached values
                        if (retrieveCachedVersion && AppConfig != null) return AppConfig;

                        var appConfig = await Get();
                        if (appConfig != null)
                        {
                            ////MK update whole config object
                            //if (AppConfig == null || appConfig.Object?.Optional != null)
                            //{
                            //    AppConfig = appConfig.Object;
                            //    if (appConfig.Object?.Optional != null)
                            //    {
                            //        currentAppConfigurationMd5Hash = JsonConvert.SerializeObject(AppConfig.Optional).ToMd5Hash();
                            //    }
                            //}
                            ////MK update just Mandatory properties
                            //else
                            //{
                            //    AppConfig.Mandatory = appConfig.Object.Mandatory;
                            //}
                            AppConfig = appConfig;
                        }
                        else
                        {
                            var alertAsync = Dialogs?.AlertAsync($"Something went wrong we couldn't retrieve your app configuration");
                            if (alertAsync != null) await alertAsync;
                        }
                    }
                }
                finally
                {
                    getAppConfigurationAutoResetEvent.Set();
                }

                cancellationToken = new CancellationTokenSource();
                await HandleRetrievedAppConfigValidation();

                return AppConfig;
            });
        }

       

        public void InitForMvvmCross()
        {
            try
            {
                // On Android extra step is required to init dialogs in MainActivity (https://github.com/aritchie/userdialogs)
                Mvx.RegisterSingleton<IUserDialogs>(() =>
                {
                    return Dialogs = UserDialogs.Instance;
                });

                isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task<AppConfiguration> Get()
        {
            if (string.IsNullOrEmpty(this.serviceEndpoint.Endpoint)) return null;

            AppConfiguration res = null;

            var appConfigHash = string.Empty;
            if (AppConfig != null)
            {
                appConfigHash = $"/{currentAppConfigurationMd5Hash}";
            }

            var endpoint = $"{this.serviceEndpoint.Endpoint}{appConfigHash}";

            try
            {
                using (var client = new HttpClient())
                {
                    var requestContent = new StringContent(JsonConvert.SerializeObject(Mapper.MappedValues));
                    requestContent.Headers.ContentType.MediaType = "application/json";
                    using (var response = await client.PostAsync(endpoint, requestContent))
                    {
                        using (var content = response.Content)
                        {
                            var responseContent = await content.ReadAsStringAsync();
                            var appConfigurationValues = JsonConvert.DeserializeObject<List<AppConfigurationValue>>(responseContent);
                            if (appConfigurationValues != null && appConfigurationValues.Any())
                            {
                                res = new AppConfiguration();
                                foreach (var value in appConfigurationValues)
                                {
                                    if (string.IsNullOrEmpty(value?.Attributes?.Name)) continue;

                                    res[value.Attributes.Name] = value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return res;
        }

        private AppConfigurationValidationResult ValidateRetrievedAppConfig(AppConfiguration appConfig = null)
        {
            var res = new AppConfigurationValidationResult();
            appConfig = appConfig ?? AppConfig;

            if (appConfig == null) return res;

            foreach (var configValue in appConfig.Values)
            {
                if (configValue.Attributes == null) continue;

                if ((configValue.Attributes?.ValueIsRequired ?? false) && configValue.Value == null)
                {
                    res.InvalidValues.Add(configValue);
                }
            }

            res.IsValid = res.InvalidValues.Count == 0;
            return res;
        }

        private async Task HandleRetrievedAppConfigValidation(AppConfigurationValidationResult validationResult = null)
        {
            if (validationResult == null)
            {
                validationResult = ValidateRetrievedAppConfig();
            }

            if (!validationResult.IsValid)
            {

                if (validationResult.InvalidValues.Count == 1)
                {
                    if (validationResult.InvalidValues[0]?.Attributes?.FailureHandler != null)
                    {
                        await validationResult.InvalidValues[0]?.Attributes?.FailureHandler.Invoke(validationResult.InvalidValues[0]);
                    }
                    else
                    {
                        var alertAsync = Dialogs?.AlertAsync($"{validationResult.InvalidValues[0]?.Attributes?.Name} is not present!");
                        if (alertAsync != null) await alertAsync;
                    }
                }
                else
                {
                    var anyBlockingAppConfiguration = validationResult.InvalidValues.Any(iv => iv.Attributes.ValueIsBlocking);

                    do
                    {
                        var alertAsync = Dialogs?.AlertAsync($"{FormatInvalidAppConfiguration(validationResult.InvalidValues)}");
                        if (alertAsync != null) await alertAsync;

                        await Task.Delay(1000);

                    } while (anyBlockingAppConfiguration && !cancellationToken.IsCancellationRequested);
                }
            }
        }

        private string FormatInvalidAppConfiguration(List<AppConfigurationValue> invalidValues)
        {
            if (invalidValues == null) return null;
            StringBuilder formattedInvalidAppConfiguration = new StringBuilder();

            foreach (var value in invalidValues)
            {
                formattedInvalidAppConfiguration.AppendLine($"{value?.Attributes?.Name} : {value?.Value}");
            }

            return formattedInvalidAppConfiguration.ToString();
        }

        public async Task BlockAppFromRunning<T>(string title, string body, Func<T> callerFunc)
        {
            do
            {
                var alertAsync = Dialogs?.AlertAsync(title);
                if (alertAsync != null) await alertAsync;

                await Task.Delay(1000);

                //Check if current app version is up-to-date
                callerFunc?.Invoke();

            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}