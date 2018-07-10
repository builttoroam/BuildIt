using System;

namespace BuildIt.Auth
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OAuthAuthorizeParameterAttribute : OAuthParameterAttribute
    {
        public OAuthAuthorizeParameterAttribute(string parameterName, bool isRequired = true)
            : base(parameterName, isRequired)
        {
        }
    }
}