using System;
using System.Threading.Tasks;
using BuildIt.Config.Core.Services.Interfaces;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

namespace BuildIt.Config.Impl.Common
{
    public class NetworkService : INetworkService
    {
        private bool isConnected;

        public NetworkService()
        {
            this.isConnected = CrossConnectivity.Current.IsConnected;

            CrossConnectivity.Current.ConnectivityChanged += OnConnectivityChanged;
        }

        public bool HasInternetConnection()
        {
            return isConnected;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs connectivityChangedEventArgs)
        {
            isConnected = connectivityChangedEventArgs.IsConnected;
        }
    }
}
