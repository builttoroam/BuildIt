using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.SqlDb
{
    internal static class DacWebServiceEndpoints
    {
        public static string GetEndpointForRegion(this AzureDbLocation region)
        {
            switch (region)
            {
                case AzureDbLocation.AustraliaEast:
                    return "https://aueprod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.AustraliaSoutheast:
                    return "https://auseprod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.NorthCentralUS:
                    return "https://ch1prod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.SouthCentralUS:
                    return "https://sn1prod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.EastUS:
                    return "https://bl2prod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.WestUS:
                    return "https://by1prod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.NorthEurope:
                    return "https://db3prod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.WestEurope:
                    return "https://am1prod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.EastAsia:
                    return "https://hkgprod-dacsvc.azure.com/dacwebservice.svc";
                case AzureDbLocation.SoutheastAsia:
                    return "https://sg1prod-dacsvc.azure.com/dacwebservice.svc";
                default:
                    throw new NotImplementedException("You have provided an unsupported region.");
            }
        }
    }
}
