using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationRequiredServices : IAppConfigurationRequiredServices
    {
        /// <summary>
        /// 
        /// </summary>
        public IAppConfigurationServiceSetup ServiceSetup { get; }
        /// <summary>
        /// 
        /// </summary>
        public IAppConfigurationEndpointService EndpointService { get; }
        /// <summary>
        /// 
        /// </summary>
        public IVersionService VersionService { get; }
        /// <summary>
        /// 
        /// </summary>
        public IUserDialogService UserDialogService { get; }
        /// <summary>
        /// 
        /// </summary>
        public INetworkService NetworkService { get; }
        /// <summary>
        /// 
        /// </summary>
        public IFileCacheService FileCacheService { get; }

        /// <summary>
        /// 
        /// </summary>
        public AppConfigurationRequiredServices(IAppConfigurationServiceSetup serviceSetup, IAppConfigurationEndpointService endpointService, IVersionService versionService, IUserDialogService userDialogService, INetworkService networkService,
                                                IFileCacheService fileCacheService)
        {
            this.FileCacheService = fileCacheService;
            this.ServiceSetup = serviceSetup;
            this.EndpointService = endpointService;
            this.VersionService = versionService;
            this.UserDialogService = userDialogService;
            this.NetworkService = networkService;
        }
    }
}
