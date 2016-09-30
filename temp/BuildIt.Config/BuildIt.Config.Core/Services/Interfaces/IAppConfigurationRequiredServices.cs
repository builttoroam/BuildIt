namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IAppConfigurationRequiredServices
    {
        IAppConfigurationServiceSetup ServiceSetup { get; }
        IAppConfigurationEndpointService EndpointService { get; }
        IVersionService VersionService { get; }
        IUserDialogService UserDialogService { get; }
        INetworkService NetworkService { get; }
    }
}
