using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.AR.FormsSamples.Models;

namespace BuildIt.AR.FormsSamples.Core.Services.Interfaces
{
    public interface IAppService
    {
        Task<IEnumerable<Organization>> GetOrganizations();
    }
}
