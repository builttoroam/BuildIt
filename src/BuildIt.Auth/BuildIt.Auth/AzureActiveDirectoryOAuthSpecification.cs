namespace BuildIt.Auth
{
    public class AzureActiveDirectoryOAuthSpecification : OAuthSpecification
    {

        public bool IsMultiTenanted { get; set; }

        public string Tenant { get; set; }

        protected virtual string BaseOAuthUrlTemplate { get; set; } = "https://login.microsoftonline.com/{0}/oauth2/{1}";

        protected virtual string BaseOAuthUrl =>
            string.Format(BaseOAuthUrlTemplate,
                IsMultiTenanted ? "common" : Tenant,
                "{0}");

        [OAuthLogoutParameter("post_logout_redirect_uri")]
        public string PostLogoutRedirectUrl { get; set; }

        protected override string BaseAuthorizeUrl => string.Format(BaseOAuthUrl, "authorize");
        protected override string BaseTokenUrl => string.Format(BaseOAuthUrl, "token");

        public override string Scope { get; set; } = "offline_access";
        protected override string BaseLogoutUrl => $"https://login.windows.net/{(IsMultiTenanted ? "common" : Tenant)}/oauth2/logout";
    }
}