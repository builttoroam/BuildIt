using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BuildIt.Auth
{
    public class OAuthManager
    {
        public OAuthSpecification Specification { get; set; }

        public string OAuthLogonUrl => Specification?.AuthorizeUrl;

        public string OAuthLogOffUrl => Specification?.LogoutUrl;

        public TokenData Token { get; private set; }

        private Action<bool> AuthenticationCompleted { get; set; }

        public void Register(Action<bool> authenticationCompleted)
        {
            AuthenticationCompleted = authenticationCompleted;
            UriLauncher.Register(UriCallback);
        }

        public void Unregister()
        {
        }

        public async Task RefreshAccessToken()
        {
            var url = Specification.TokenUrl;

            var post = Specification.RefreshTokenPostData;

            var token = Token.RefreshToken;
            post["refresh_token"] = token;

            var content = new FormUrlEncodedContent(post);

            using (var client = new HttpClient())
            {
                var result = await client.PostAsync(new Uri(url), content);
                var output = await result.Content.ReadAsStringAsync();
                Debug.WriteLine(result != null);

                Token = JsonConvert.DeserializeObject<TokenData>(output);
                Debug.WriteLine(result != null);
            }
        }

        private async void UriCallback(Uri uri)
        {
            try
            {
                Debug.WriteLine(uri != null);

                var arguments = uri?.Query
                    .Substring(1) // Remove '?'
                    .Split('&')
                    .Select(q => q.Split('='))
                    .ToDictionary(q => q.FirstOrDefault(), q => q.Skip(1).FirstOrDefault());
                var code = arguments?["code"];

                var url = Specification.TokenUrl;

                Specification.Code = code;

                var post = Specification.TokenPostData;

                var content = new FormUrlEncodedContent(post);

                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(new Uri(url), content);
                    var output = await result.Content.ReadAsStringAsync();
                    Debug.WriteLine(result != null);

                    Token = JsonConvert.DeserializeObject<TokenData>(output);
                    Debug.WriteLine(result != null);
                }

                AuthenticationCompleted?.Invoke(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                AuthenticationCompleted?.Invoke(false);
            }
        }
    }
}