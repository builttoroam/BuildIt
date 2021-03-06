using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BuildIt.Auth
{
    public abstract class OAuthSpecification
    {
        [OAuthAuthorizeParameter("client_id")]
        [OAuthTokenParameter("client_id")]
        [OAuthRefreshTokenParameter("client_id")]
        public string ClientId { get; set; }

        [OAuthAuthorizeParameter("redirect_uri")]
        [OAuthTokenParameter("redirect_uri")]
        public string RedirectUri { get; set; }

        [OAuthAuthorizeParameter("scope")]
        public abstract string Scope { get; set; }

        [OAuthAuthorizeParameter("resource", isRequired: false)]
        [OAuthTokenParameter("resource")]
        public string Resource { get; set; }

        [OAuthAuthorizeParameter("state", isRequired: false)]
        public string State { get; set; }

        [OAuthAuthorizeParameter("nounce", isRequired: false)]
        public string Nounce { get; set; }

        [OAuthTokenParameter("code")]
        public string Code { get; set; }

        public string AuthorizeUrl
        {
            get
            {
                var queryParameters = QueryParameters<OAuthAuthorizeParameterAttribute>();
                return string.IsNullOrWhiteSpace(queryParameters) ? BaseAuthorizeUrl : $"{BaseAuthorizeUrl}?{queryParameters}";
            }
        }

        public string TokenUrl => BaseTokenUrl;

        public string LogoutUrl
        {
            get
            {
                var queryParameters = QueryParameters<OAuthLogoutParameterAttribute>();
                return string.IsNullOrWhiteSpace(queryParameters) ? BaseLogoutUrl : $"{BaseLogoutUrl}?{queryParameters}";
            }
        }

        public IDictionary<string, string> TokenPostData => ParameterValues<OAuthTokenParameterAttribute>();

        public IDictionary<string, string> RefreshTokenPostData => ParameterValues<OAuthRefreshTokenParameterAttribute>();

        [OAuthTokenParameter("grant_type")]
        protected string GrantType { get; set; } = "authorization_code";

        [OAuthRefreshTokenParameter("grant_type")]
        protected string RefreshTokenGrantType { get; set; } = "refresh_token";

        [OAuthAuthorizeParameter("response_type")]
        protected string ResponseType { get; set; } = "code";

        protected abstract string BaseAuthorizeUrl { get; }

        protected abstract string BaseTokenUrl { get; }

        protected abstract string BaseLogoutUrl { get; }

        private IDictionary<string, string> ParameterValues<TParameter>()
            where TParameter : OAuthParameterAttribute
        {
            var props =
            (from p in GetType().GetRuntimeProperties()
             let oauthparam = ((MemberInfo)p).GetCustomAttribute<TParameter>()
             where oauthparam != null
             let pvalue = p.GetValue(this) + string.Empty
             where !string.IsNullOrWhiteSpace(pvalue) || oauthparam.IsRequired
             select new { oauthparam.ParameterName, pvalue }).ToDictionary(x => x.ParameterName, x => x.pvalue);
            return props;
        }

        private string QueryParameters<TParameter>()
            where TParameter : OAuthParameterAttribute
        {
            var values = ParameterValues<TParameter>();
            var parameters = from pvalue in values
                             let encoded = Uri.EscapeDataString(pvalue.Value)
                             select $"{pvalue.Key}={encoded}";
            return string.Join("&", parameters);
        }
    }
}