using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Core.Utilities;
using Newtonsoft.Json;

namespace BuildIt.Config.Core.Services
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly IAppConfigurationEndpointService endpointService;
        private readonly INetworkService networkService;

        private readonly AutoResetEvent getAppConfigurationAutoResetEvent = new AutoResetEvent(true);

        private string currentAppConfigurationMd5Hash;

        public IUserDialogService UserDialogService { get; }
        public IVersionService VersionService { get; }

        public AppConfigurationMapper Mapper { get; } = new AppConfigurationMapper();
        public AppConfiguration AppConfig { get; private set; }

        public List<KeyValuePair<string, string>> AdditionalHeaders { get; set; } = new List<KeyValuePair<string, string>>();

        public AppConfigurationService(IAppConfigurationEndpointService endpointService, IVersionService versionService, IUserDialogService userDialogService, INetworkService networkService)
        {
            this.endpointService = endpointService;
            this.networkService = networkService;

            this.UserDialogService = userDialogService;
            this.VersionService = versionService;
        }

        public async Task<AppConfiguration> LoadAppConfig(bool handleLoadValidation = true, bool retrieveCachedVersion = true)
        {
            if (AppConfig != null) return AppConfig;
            return await RetrieveAppConfig(handleLoadValidation, retrieveCachedVersion);
        }

        private async Task<AppConfiguration> RetrieveAppConfig(bool handleRetrievalValidation = true, bool retrieveCachedVersion = true)
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

                        var appConfigurationServerResponse = await Get();
                        AppConfig = CreateAppConfigurationOutOfServerResponse(appConfigurationServerResponse);
                        if (handleRetrievalValidation)
                        {
                            if (appConfigurationServerResponse.HasErrors)
                            {
                                await HandleRetrievedAppConfigFailure(appConfigurationServerResponse);
                            }
                            else
                            {
                                await HandleRetrievedAppConfigValidation();
                            }
                        }
                    }
                }
                finally
                {
                    getAppConfigurationAutoResetEvent.Set();
                }

                return AppConfig;
            });
        }

        private AppConfiguration CreateAppConfigurationOutOfServerResponse(AppConfigurationServerResponse appConfigurationServerResponse)
        {
            AppConfiguration res = null;
            if ((appConfigurationServerResponse?.HasErrors ?? true) || !appConfigurationServerResponse.HasConfigValues) return res;

            res = new AppConfiguration();
            foreach (var value in appConfigurationServerResponse.AppConfigValues)
            {
                if (string.IsNullOrEmpty(value?.Attributes?.Name)) continue;

                res[value.Attributes.Name] = value;
            }

            return res;
        }

        private async Task<AppConfigurationServerResponse> Get()
        {
            if (string.IsNullOrEmpty(this.endpointService.Endpoint)) return null;

            AppConfigurationServerResponse res = null;

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
                    //TODO: once we have BuildIt.General referenced, refactor this code to use for instance DoForEach 
                    if (AdditionalHeaders != null)
                    {
                        foreach (var extraHeaderKeyValuePair in AdditionalHeaders)
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
                            res = JsonConvert.DeserializeObject<AppConfigurationServerResponse>(responseContent);
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                await UserDialogService.AlertAsync($"{Strings.OpsSomethingWentWrong}: {JsonConvert.SerializeObject(e)}");
#endif
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
        private async Task HandleRetrievedAppConfigFailure(AppConfigurationServerResponse appConfigurationServerResponse)
        {
#if DEBUG
            //Display all errors
            var responseErrors = appConfigurationServerResponse.AppConfigErors;
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

