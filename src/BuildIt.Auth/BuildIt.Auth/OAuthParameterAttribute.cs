using System;

namespace BuildIt.Auth
{
    public abstract class OAuthParameterAttribute : Attribute
    {
        protected OAuthParameterAttribute(string parameterName, bool isRequired = true)
        {
            ParameterName = parameterName;
            IsRequired = isRequired;
        }

        public string ParameterName { get; }

        public bool IsRequired { get; }
    }
}