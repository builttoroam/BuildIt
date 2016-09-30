using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Core.Services
{
    public class AppConfigurationRequiredServices : IAppConfigurationRequiredServices
    {
        public IAppConfigurationServiceSetup ServiceSetup { get; }
        public IAppConfigurationEndpointService EndpointService { get; }
        public IVersionService VersionService { get; }
        public IUserDialogService UserDialogService { get; }
        public INetworkService NetworkService { get; }

        public AppConfigurationRequiredServices(IAppConfigurationServiceSetup serviceSetup, IAppConfigurationEndpointService endpointService, IVersionService versionService, IUserDialogService userDialogService, INetworkService networkService)
        {
            this.ServiceSetup = serviceSetup;
            this.EndpointService = endpointService;            
            this.VersionService = versionService;
            this.UserDialogService = userDialogService;
            this.NetworkService = networkService;
        }
    }
}
