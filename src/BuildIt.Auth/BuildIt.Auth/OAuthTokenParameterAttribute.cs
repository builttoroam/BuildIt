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

    [AttributeUsage(AttributeTargets.Property)]
    public class OAuthRefreshTokenParameterAttribute : OAuthParameterAttribute
    {
        public OAuthRefreshTokenParameterAttribute(string parameterName, bool isRequired = true)
            : base(parameterName, isRequired)
        {
        }
    }

}