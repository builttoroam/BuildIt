namespace BuildIt.Auth
{
    public class GoogleOAuthSpecification : OAuthSpecification
    {
        protected override string BaseAuthorizeUrl => "https://accounts.google.com/o/oauth2/v2/auth";
        protected override string BaseTokenUrl => "https://www.googleapis.com/oauth2/v4/token";

        public override string Scope { get; set; } = "profile";
    }
}