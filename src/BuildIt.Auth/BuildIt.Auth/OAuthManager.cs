using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            //AuthenticationCompleted = null;
            //UriLauncher.Unregister();
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

        public async Task RefreshAccessToken()
        {
            //        POST /oauth2/v4/token HTTP/1.1
            //Host: www.googleapis.com
            //Content-Type: application/x-www-form-urlencoded

            //client_id = 8819981768.apps.googleusercontent.com &
            //client_secret = your_client_secret &
            //refresh_token = 1 / 6BMfW9j53gdGImsiyUH5kU5RsR4zwI9lUVX-tqf8JXQ&
            //grant_type=refresh_token

            var url = Specification.TokenUrl;

            var post = Specification.RefreshTokenPostData;

            var token = Token.refresh_token;
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
    }
}