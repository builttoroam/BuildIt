using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Auth;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Authentication
{
    public partial class MainPage : ContentPage
    {
        private OAuthManager AzureManager { get; }
            = new OAuthManager
            {
                Specification
                    = new AzureActiveDirectoryOAuthSpecification
                    {
                        ClientId= "5db87179-0079-4264-a325-32be8cea7117",
                        RedirectUri = "ext.auth://callback",
                        PostLogoutRedirectUrl = "ext.auth://callback",
                        Tenant = "nicksdemodir.onmicrosoft.com",
                        IsMultiTenanted = false,
                        State = "12345",
                        Nounce = "7362CAEA-9CA5-4B43-9BA3-34D7C303EBA7",
                        Resource= "https://graph.microsoft.com",
                    }
            };
        
        private OAuthManager GoogleManager { get; }
             = new OAuthManager
             {
                 Specification
                        = new GoogleOAuthSpecification
                        {
                            ClientId = "966274654419-2s5tgbb717ecev48ghn46ij0p8qp90nf.apps.googleusercontent.com",
                            RedirectUri = "ext.auth:/callback",
                            Scope= "email profile"
                        }
             };

        public MainPage()
        {
            InitializeComponent();
        }

        //private string MicrosoftAuthorizationLink =>
        //    "https://login.microsoftonline.com/nicksdemodir.onmicrosoft.com/oauth2/authorize?" +
        //    "client_id=5db87179-0079-4264-a325-32be8cea7117&" +
        //    "response_type=code&" +
        //    "redirect_uri=extauth%3A%2F%2Fcallback&" +
        //    "scope=offline_acesss&" +
        //    "state=12345&" +
        //    "nonce=7362CAEA-9CA5-4B43-9BA3-34D7C303EBA7&" +
        //    "resource=https%3A%2F%2Fgraph.microsoft.com";



        //private string MicrosoftTokenUrl => "https://login.microsoftonline.com/nicksdemodir.onmicrosoft.com/oauth2/token";

        //private IDictionary<string,string> MicrosoftTokenPost(string code)
        //{
        //    return ("grant_type=authorization_code&" +
        //           "client_id=5db87179-0079-4264-a325-32be8cea7117&" +
        //           "redirect_uri=extauth://callback&" +
        //           "resource=https://graph.microsoft.com&" +
        //            $"code={code}").Split('&')
        //                  .Select(q => q.Split('='))
        //                  .ToDictionary(q => q.FirstOrDefault(), q => q.Skip(1).FirstOrDefault());
        //}


        //private string GoogleAuthorizationLink =>
        //    "https://accounts.google.com/o/oauth2/v2/auth?" +
        //    "scope=email%20profile&" +
        //    "redirect_uri=ext.auth:/callback&" +
        //    "response_type=code&" +
        //    "client_id=966274654419-2s5tgbb717ecev48ghn46ij0p8qp90nf.apps.googleusercontent.com";


        //private string GoogleTokenUrl => "https://www.googleapis.com/oauth2/v4/token";

        //private IDictionary<string, string> GoogleTokenPost(string code)
        //{
        //    return (
        //        $"code={code}&" +
        //        "client_id=966274654419-2s5tgbb717ecev48ghn46ij0p8qp90nf.apps.googleusercontent.com&" +
        //        "redirect_uri=ext.auth:/callback&" +
        //        "grant_type=authorization_code"
        //                ).Split('&')
        //                  .Select(q => q.Split('='))
        //                  .ToDictionary(q => q.FirstOrDefault(), q => q.Skip(1).FirstOrDefault());
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();


            //UriLauncher.Register(UriCallback);
        }

        private void AzureAuthenticated(bool authenticated)
        {
            Debug.WriteLine($"Azure authenticated - {authenticated}");
        }
        private void GoogleAuthenticated(bool authenticated)
        {
            Debug.WriteLine($"Google authenticated - {authenticated}");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            AzureManager.Unregister();
            GoogleManager.Unregister();
            //UriLauncher.Unregister();
        }

        //private async void UriCallback(Uri uri)
        //{
        //    Debug.WriteLine(uri != null);

        //    var isMicrosoft = false;
        //    if (uri.AbsoluteUri.Contains("extauth"))
        //    {
        //        isMicrosoft = true;
        //    }

        //    var arguments = uri?.Query
        //                  .Substring(1) // Remove '?'
        //                  .Split('&')
        //                  .Select(q => q.Split('='))
        //                  .ToDictionary(q => q.FirstOrDefault(), q => q.Skip(1).FirstOrDefault());
        //    var code = arguments?["code"];

        //    var url =isMicrosoft?  MicrosoftTokenUrl:GoogleTokenUrl;
        //    var post = isMicrosoft ? MicrosoftTokenPost(code) : GoogleTokenPost(code);


        //    var content = new FormUrlEncodedContent(post);

        //    using (var client = new HttpClient())
        //    {
        //        var result = await client.PostAsync(new Uri(url), content);
        //        var output = await result.Content.ReadAsStringAsync();
        //        Debug.WriteLine(result!=null);

        //        var tokenInfo = JsonConvert.DeserializeObject<TokenData>(output);
        //        Debug.WriteLine(result!=null);
        //    }
        //}

        private void Button_Clicked(object sender, EventArgs e)
        {
            //Device.OpenUri(new Uri(MicrosoftAuthorizationLink));
            AzureManager.Register(AzureAuthenticated);
            Device.OpenUri(new Uri(AzureManager.OAuthLogonUrl));
        }

        private void ButtonGoogle_Clicked(object sender, EventArgs e)
        {
            //Device.OpenUri(new Uri(GoogleAuthorizationLink));
            GoogleManager.Register(GoogleAuthenticated);
            Device.OpenUri(new Uri ( GoogleManager.OAuthLogonUrl));
        }

        private async void ButtonAzureRefresh_Clicked(object sender, EventArgs e)
        {
            await AzureManager.RefreshAccessToken();
        }

        private async void ButtonGoogleRefresh_Clicked(object sender, EventArgs e)
        {
            await GoogleManager.RefreshAccessToken();
        }

        private void ButtonAzureLogout_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(AzureManager.OAuthLogOffUrl));
        }

        private void ButtonGoogleLogout_Clicked(object sender, EventArgs e)
        {
            //Device.OpenUri(new Uri($"https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue={WebUtility.UrlEncode("ext.auth:/callback")}"));
            Device.OpenUri(new Uri($"{GoogleManager.OAuthLogOffUrl}?token={WebUtility.UrlEncode(GoogleManager.Token.access_token)}"));
        }
    }
}
