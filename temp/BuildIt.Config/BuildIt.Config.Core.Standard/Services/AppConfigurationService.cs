using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Models;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Newtonsoft.Json;
using BuildIt;
using BuildIt.Config.Core.Standard.Utilities;

namespace BuildIt.Config.Core.Standard.Services
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly IAppConfigurationEndpointService endpointService;

        private readonly AutoResetEvent getAppConfigurationAutoResetEvent = new AutoResetEvent(true);

        private string currentAppConfigurationMd5Hash;

        public IUserDialogService UserDialogService { get; }
        public IVersionService VersionService { get; }
        
        public AppConfigurationMapper Mapper { get; } = new AppConfigurationMapper();
        public AppConfiguration AppConfig { get; private set; }

        public List<KeyValuePair<string, string>> ExtraHeaders { get; set; } = new List<KeyValuePair<string, string>>();

        public AppConfigurationService(IAppConfigurationEndpointService endpointService, IVersionService versionService, IUserDialogService userDialogService)
        {
            this.endpointService = endpointService;
            this.UserDialogService = userDialogService;
            this.VersionService = versionService;
        }

        public async Task<AppConfiguration> LoadAppConfig(bool retrieveCachedVersion = true)
        {
            if (AppConfig != null) return AppConfig;
            return await RetrieveAppConfig(retrieveCachedVersion);
        }

        private async Task<AppConfiguration> RetrieveAppConfig(bool retrieveCachedVersion = true)
        {
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
                            var alertAsync = UserDialogService?.AlertAsync(Constants.AppConfigurationNotFoundError);
                            if (alertAsync != null) await alertAsync;
                        }
                    }
                }
                finally
                {
                    getAppConfigurationAutoResetEvent.Set();
                }

                await HandleRetrievedAppConfigValidation();

                return AppConfig;
            });
        }

        private async Task<AppConfiguration> Get()
        {
            if (string.IsNullOrEmpty(this.endpointService.Endpoint)) return null;

            AppConfiguration res = null;

            var appConfigHash = string.Empty;
            if (AppConfig != null)
            {
                appConfigHash = $"/{currentAppConfigurationMd5Hash}";
            }

            var endpoint = $"{this.endpointService.Endpoint}{appConfigHash}";

            try
            {
                using (var client = new HttpClient())
                {
                    //TODO: once we have BuildIt.General refactor this code to use for instance DoForEach 
                    if (ExtraHeaders != null)
                    {
                        foreach (var extraHeaderKeyValuePair in ExtraHeaders)
                        {
                            client.DefaultRequestHeaders.Add(extraHeaderKeyValuePair.Key, extraHeaderKeyValuePair.Value);
                        }
                    }

                    var requestContent = new StringContent(JsonConvert.SerializeObject(Mapper.MappedValues));
                    requestContent.Headers.ContentType.MediaType = "application/json";
                    using (var response = await client.PostAsync(endpoint, requestContent))
                    {
                        if (response == null) return res;
                        using (var content = response.Content)
                        {
                            var responseContent = await content.ReadAsStringAsync();
                            var appConfigurationResponse = JsonConvert.DeserializeObject<AppConfigurationResponse>(responseContent);

                            if (appConfigurationResponse.HasErrors)
                            {
#if DEBUG
                                //Display all errors
                                var responseErrors = appConfigurationResponse.AppConfigErors;
                                foreach (var responseError in responseErrors)
                                {
                                    await UserDialogService.AlertAsync($"Message: {responseError.Content}");
                                }
#else
    //Display user-friendly alert
                                    var alertAsync = UserDialogService?.AlertAsync($"Something went wrong we couldn't retrieve your app configuration");
                                    if (alertAsync != null) await alertAsync;
#endif
                            }
                            else if (response.StatusCode != HttpStatusCode.OK)
                            {
                                await UserDialogService.AlertAsync(Strings.OpsSomethingWentWrong);
                            }
                            else
                            {
                                if (appConfigurationResponse.HasConfigValues)
                                {
                                    res = new AppConfiguration();
                                    foreach (var value in appConfigurationResponse.AppConfigValues)
                                    {
                                        if (string.IsNullOrEmpty(value?.Attributes?.Name)) continue;

                                        res[value.Attributes.Name] = value;
                                    }
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

        private AppConfigurationValidationResult ValidateRetrievedAppConfig()
        {
            var res = new AppConfigurationValidationResult();

            if (AppConfig == null) return res;

            foreach (var configValue in AppConfig.Values)
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

        private async Task HandleRetrievedAppConfigValidation()
        {
            var validationResult = ValidateRetrievedAppConfig();
            if (!validationResult.IsValid)
            {
                if (!validationResult.InvalidValues.Any()) return;

                var anyBlockingAppConfiguration = validationResult.InvalidValues.Any(iv => iv.Attributes.ValueIsBlocking);

                if (validationResult.InvalidValues.Count == 1)
                {
                    if (validationResult.InvalidValues[0]?.Attributes?.FailureHandler != null)
                    {
                        await validationResult.InvalidValues[0]?.Attributes?.FailureHandler.Invoke(validationResult.InvalidValues[0]);
                        return;
                    }
                    else
                    {
                        await UserDialogService.AlertAsync($"{validationResult.InvalidValues[0]?.Attributes?.Name} is not present!");
                    }
                }
                else
                {
                    await UserDialogService.AlertAsync($"{FormatInvalidAppConfiguration(validationResult.InvalidValues)}");
                }

                if (anyBlockingAppConfiguration)
                {
                    await RetrieveAppConfig(false);
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
    }
}