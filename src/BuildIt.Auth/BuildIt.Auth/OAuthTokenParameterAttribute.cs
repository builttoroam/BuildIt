using System;

namespace BuildIt.Auth
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OAuthTokenParameterAttribute : OAuthParameterAttribute
    {
        public OAuthTokenParameterAttribute(string parameterName, bool isRequired = true)
            : base(parameterName, isRequired)
        {
        }
    }
}