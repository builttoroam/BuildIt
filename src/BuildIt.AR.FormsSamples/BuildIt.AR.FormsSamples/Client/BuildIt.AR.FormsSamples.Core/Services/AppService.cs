using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using BuildIt.AR.FormsSamples.Models;
using BuildIt.AR.FormsSamples.Client;

namespace BuildIt.AR.FormsSamples.Core.Services
{
    public class AppService : IAppService
    {
        private IAppServiceAPI ApiService { get; }
        public AppService(IAppServiceAPI apiService)
        {
            ApiService = apiService;
        }
        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            return await ApiService.ApiOrganizationGetAsync();
        }
    }
}
