using System;

namespace BuildIt.Auth
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OAuthLogoutParameterAttribute : OAuthParameterAttribute
    {
        public OAuthLogoutParameterAttribute(string parameterName, bool isRequired = true)  : base(parameterName, isRequired)
        {
        }
    }
}
