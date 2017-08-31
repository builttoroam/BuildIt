using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BuildIt;

namespace BuildIt.AR.FormsSamples.Core
{
    public class EnvironmentConfiguration : BaseConfiguration
    {
        public EnvironmentConfiguration(IDictionary<Expression<Func<string>>, string> initializers = null) :
            base(initializers)
        {
        }

        public string BaseUri => Value();

        public string ApplicationInsightsAppId
        {
#if DEBUG
            get => Guid.Empty.ToString();
#else
            get => Value();
#endif
            set => Data[nameof(ApplicationInsightsAppId)] = value;
        }

        public string ApplicationInsightsSecretKey
        {
            get => Value();
            set => Data[nameof(ApplicationInsightsSecretKey)] = value;
        }
    }
}
