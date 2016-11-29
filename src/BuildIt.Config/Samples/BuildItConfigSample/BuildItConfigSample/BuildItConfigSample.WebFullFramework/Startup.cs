using System;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BuildItConfigSample.WebFullFramework.Startup))]
namespace BuildItConfigSample.WebFullFramework
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CreateEnvironmentVariables();
            ConfigureAuth(app);
        }

        private void CreateEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("App_VersionInfo_CurrentAppVersion", "1.0.0");
            Environment.SetEnvironmentVariable("App_VersionInfo_MinimumAppVersion", "1.0.1");
            Environment.SetEnvironmentVariable("App_ServiceNotification_Title", "Some title");
            Environment.SetEnvironmentVariable("App_States", "[{\"FullName\":\"Scotland\",\"ShortCode\":\"SCO\",\"StateId\":1},{\"FullName\":\"North\",\"ShortCode\":\"NOR\",\"StateId\":13},{\"FullName\":\"Midlands\",\"ShortCode\":\"MID\",\"StateId\":14},{\"FullName\":\"Wales\",\"ShortCode\":\"WAL\",\"StateId\":4},{\"FullName\":\"South East\",\"ShortCode\":\"SEA\",\"StateId\":16},{\"FullName\":\"South West\",\"ShortCode\":\"SWE\",\"StateId\":17}]");
        }
    }
}
