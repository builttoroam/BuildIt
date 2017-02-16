// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{
    /// <summary>
    /// The Search API provides a similar (but not exact) experience to
    /// Bing.com/Search by returning search results that Bing determines are
    /// relevant to the specified query. The results also identify the order
    /// that you must display the content in (see Using Ranking to Display
    /// Results). The response may also include related search links and
    /// suggest a query string that may more accurately represent the user's
    /// intent. Typically, you will call this API instead of calling the
    /// other APIs in the Bing API family, such as the Image API or News API.
    /// </summary>
    public partial class WebSearchAPIV5 : Microsoft.Rest.ServiceClient<WebSearchAPIV5>, IWebSearchAPIV5
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        public System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public Newtonsoft.Json.JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public Newtonsoft.Json.JsonSerializerSettings DeserializationSettings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the WebSearchAPIV5 class.
        /// </summary>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public WebSearchAPIV5(params System.Net.Http.DelegatingHandler[] handlers) : base(handlers)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the WebSearchAPIV5 class.
        /// </summary>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public WebSearchAPIV5(System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : base(rootHandler, handlers)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the WebSearchAPIV5 class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public WebSearchAPIV5(System.Uri baseUri, params System.Net.Http.DelegatingHandler[] handlers) : this(handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            this.BaseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the WebSearchAPIV5 class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public WebSearchAPIV5(System.Uri baseUri, System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : this(rootHandler, handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            this.BaseUri = baseUri;
        }

        /// <summary>
        /// An optional partial-method to perform custom initialization.
        ///</summary> 
        partial void CustomInitialize();
        /// <summary>
        /// Initializes client properties.
        /// </summary>
        private void Initialize()
        {
            this.BaseUri = new System.Uri("https://api.cognitive.microsoft.com/bing/v5.0");
            SerializationSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new System.Collections.Generic.List<Newtonsoft.Json.JsonConverter>
                    {
                        new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter()
                    }
            };
            DeserializationSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new System.Collections.Generic.List<Newtonsoft.Json.JsonConverter>
                    {
                        new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter()
                    }
            };
            CustomInitialize();
        }
        /// <summary>
        /// Get web, image, news, &amp; videos results for a given query.
        /// </summary>
        /// <param name='count'>
        /// The number of search results to return in the response. The actual number
        /// delivered may be less than requested.
        /// </param>
        /// <param name='offset'>
        /// The zero-based offset that indicates the number of search results to skip
        /// before returning results.
        /// </param>
        /// <param name='mkt'>
        /// The market where the results come from. Typically, this is the country
        /// where the user is making the request from; however, it could be a
        /// different country if the user is not located in a country where Bing
        /// delivers results. The market must be in the form {language code}-{country
        /// code}. For example, en-US.
        /// 
        /// &lt;br&gt;
        /// &lt;br&gt;
        /// Full list of supported markets:
        /// &lt;br&gt;
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US.
        /// Possible values include: 'en-us'
        /// </param>
        /// <param name='safesearch'>
        /// A filter used to filter results for adult content. Possible values
        /// include: 'Off', 'Moderate', 'Strict'
        /// </param>
        /// <param name='subscriptionKey'>
        /// subscription key in url
        /// </param>
        /// <param name='ocpApimSubscriptionKey'>
        /// subscription key in header
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> SearchWithHttpMessagesAsync(string query, double? count = 10, double? offset = 0, string mkt = "en-us", string safesearch = "Moderate", string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            string q = query;
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("q", q);
                tracingParameters.Add("count", count);
                tracingParameters.Add("offset", offset);
                tracingParameters.Add("mkt", mkt);
                tracingParameters.Add("safesearch", safesearch);
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "Search", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "search").ToString();
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
            if (q != null)
            {
                _queryParameters.Add(string.Format("q={0}", System.Uri.EscapeDataString(q)));
            }
            if (count != null)
            {
                _queryParameters.Add(string.Format("count={0}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(count, this.SerializationSettings).Trim('"'))));
            }
            if (offset != null)
            {
                _queryParameters.Add(string.Format("offset={0}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(offset, this.SerializationSettings).Trim('"'))));
            }
            if (mkt != null)
            {
                _queryParameters.Add(string.Format("mkt={0}", System.Uri.EscapeDataString(mkt)));
            }
            if (safesearch != null)
            {
                _queryParameters.Add(string.Format("safesearch={0}", System.Uri.EscapeDataString(safesearch)));
            }
            if (subscriptionKey != null)
            {
                _queryParameters.Add(string.Format("subscription-key={0}", System.Uri.EscapeDataString(subscriptionKey)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            System.Net.Http.HttpRequestMessage _httpRequest = new System.Net.Http.HttpRequestMessage();
            System.Net.Http.HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new System.Net.Http.HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers
            if (ocpApimSubscriptionKey != null)
            {
                if (_httpRequest.Headers.Contains("Ocp-Apim-Subscription-Key"))
                {
                    _httpRequest.Headers.Remove("Ocp-Apim-Subscription-Key");
                }
                _httpRequest.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);
            }
            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            // Send Request
            if (_shouldTrace)
            {
                Microsoft.Rest.ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await this.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                Microsoft.Rest.ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            System.Net.HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 401 && (int)_statusCode != 403 && (int)_statusCode != 429)
            {
                var ex = new Microsoft.Rest.HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null)
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    _responseContent = string.Empty;
                }
                ex.Request = new Microsoft.Rest.HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new Microsoft.Rest.HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_shouldTrace)
                {
                    Microsoft.Rest.ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new Microsoft.Rest.HttpOperationResponse();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            if (_shouldTrace)
            {
                Microsoft.Rest.ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

    }
}
