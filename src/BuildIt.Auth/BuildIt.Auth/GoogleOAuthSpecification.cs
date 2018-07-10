namespace BuildIt.Auth
{
    public class GoogleOAuthSpecification : OAuthSpecification
    {
        public override string Scope { get; set; } = "profile";

        protected override string BaseAuthorizeUrl => "https://accounts.google.com/o/oauth2/v2/auth";

        protected override string BaseTokenUrl => "https://www.googleapis.com/oauth2/v4/token";

        protected override string BaseLogoutUrl => "https://accounts.google.com/o/oauth2/revoke";
    }
}