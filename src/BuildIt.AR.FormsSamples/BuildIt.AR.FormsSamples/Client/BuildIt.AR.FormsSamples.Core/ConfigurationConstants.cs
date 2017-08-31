using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BuildIt.AR.FormsSamples.Core.Services;

namespace BuildIt.AR.FormsSamples.Core
{
    public static partial class Constants
    {
        private static readonly EnvironmentConfiguration Config = null;


        public const EnvironmentConfigurations CurrentConfiguration = EnvironmentConfigurations.Local;
        // Dev // public const BuildIt.AR.FormsSamplesConfigurations CurrentConfiguration = BuildIt.AR.FormsSamplesConfigurations.Dev;
        // Test // public const BuildIt.AR.FormsSamplesConfigurations CurrentConfiguration = BuildIt.AR.FormsSamplesConfigurations.Test;
        // Prod // public const BuildIt.AR.FormsSamplesConfigurations CurrentConfiguration =BuildIt.AR.FormsSamplesConfigurations.Production;

        public static IDictionary<EnvironmentConfigurations, EnvironmentConfiguration> Configurations =
                new Dictionary<EnvironmentConfigurations, EnvironmentConfiguration>
                {
                    {
                        EnvironmentConfigurations.Local,
                        new EnvironmentConfiguration
                        (
                            new Dictionary<Expression<Func<string>>, string>
                            {
                                {
                                    () => Config.BaseUri, "http://localhost"
                                }
                            }
                        )
                    },
                    {
                        EnvironmentConfigurations.Dev, new EnvironmentConfiguration
                        (
                            new Dictionary<Expression<Func<string>>, string>
                            {
                                {
                                    () => Config.BaseUri, "http://dev.azurewebsites.net/"
                                }
                            }
                        )
                    },
                    {
                        EnvironmentConfigurations.Test, new EnvironmentConfiguration
                        (
                            new Dictionary<Expression<Func<string>>, string>
                            {
                                {
                                    () => Config.BaseUri, "http://test.azurewebsites.net/"
                                }
                            }
                        )
                    },
                    {
                        EnvironmentConfigurations.Prod, new EnvironmentConfiguration
                        (
                            new Dictionary<Expression<Func<string>>, string>
                            {
                                {
                                    () => Config.BaseUri, "http://test.azurewebsites.net/"
                                }
                            }
                        )
                    }
                };
    }
}
