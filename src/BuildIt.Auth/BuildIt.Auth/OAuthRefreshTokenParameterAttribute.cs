using System;

namespace BuildIt.Auth
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OAuthRefreshTokenParameterAttribute : OAuthParameterAttribute
    {
        public OAuthRefreshTokenParameterAttribute(string parameterName, bool isRequired = true)
            : base(parameterName, isRequired)
        {
        }
    }
}