using System;

namespace BuildIt.Auth
{
    public abstract class OAuthParameterAttribute : Attribute
    {
        public string ParameterName { get; }
        public bool IsRequired { get; }

        protected OAuthParameterAttribute(string parameterName, bool isRequired = true)
        {
            ParameterName = parameterName;
            IsRequired = isRequired;
        }
    }
}