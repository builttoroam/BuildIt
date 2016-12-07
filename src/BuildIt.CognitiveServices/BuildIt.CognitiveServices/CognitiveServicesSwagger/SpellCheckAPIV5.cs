// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{
    using Microsoft.Rest;

    public partial class SpellCheckAPIV5 : Microsoft.Rest.ServiceClient<SpellCheckAPIV5>, ISpellCheckAPIV5
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
        /// Initializes a new instance of the SpellCheckAPIV5 class.
        /// </summary>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public SpellCheckAPIV5(params System.Net.Http.DelegatingHandler[] handlers) : base(handlers)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the SpellCheckAPIV5 class.
        /// </summary>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public SpellCheckAPIV5(System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : base(rootHandler, handlers)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the SpellCheckAPIV5 class.
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
        public SpellCheckAPIV5(System.Uri baseUri, params System.Net.Http.DelegatingHandler[] handlers) : this(handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            this.BaseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the SpellCheckAPIV5 class.
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
        public SpellCheckAPIV5(System.Uri baseUri, System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : this(rootHandler, handlers)
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
            this.BaseUri = new System.Uri("https://api.cognitive.microsoft.com/bing/v5.0/spellcheck");
            SerializationSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new  System.Collections.Generic.List<Newtonsoft.Json.JsonConverter>
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
        /// <param name='mode'>
        /// Mode of spellcheck:
        /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like spelling
        /// corrections. It can correct long queries, provide casing corrections and
        /// suppresses aggressive corrections.&lt;/li&gt;
        /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
        /// corrections. It will correct small queries(up to length 9 tokens) without
        /// any casing changes and will be more optimized (perf and relevance)
        /// towards search like queries.&lt;/li&gt;&lt;/ul&gt;
        /// . Possible values include: 'spell', 'proof'
        /// </param>
        /// <param name='mkt'>
        /// For proof mode, only support en-us, es-es, pt-br,
        /// For spell mode, support all language codes. Possible values include:
        /// 'en-us', 'es-es', 'pt-br'
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
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> SpellCheckWithHttpMessagesAsync(string mode = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("mode", mode);
                tracingParameters.Add("mkt", mkt);
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "SpellCheck", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "").ToString();
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
            if (mode != null)
            {
                _queryParameters.Add(string.Format("mode={0}", System.Uri.EscapeDataString(mode)));
            }
            if (mkt != null)
            {
                _queryParameters.Add(string.Format("mkt={0}", System.Uri.EscapeDataString(mkt)));
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
            _httpRequest.Method = new System.Net.Http.HttpMethod("POST");
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
                foreach(var _header in customHeaders)
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
            if ((int)_statusCode != 200 && (int)_statusCode != 400 && (int)_statusCode != 401 && (int)_statusCode != 403 && (int)_statusCode != 429)
            {
                var ex = new Microsoft.Rest.HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null) {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else {
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

        /// <param name="requestBody">
        /// Input content
        /// </param>
        /// <param name="text">
        /// Input content
        /// </param>
        /// <param name='mode'>
        /// Mode of spellcheck:
        /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like spelling
        /// corrections. It can correct long queries, provide casing corrections and
        /// suppresses aggressive corrections.&lt;/li&gt;
        /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
        /// corrections. It will correct small queries(up to length 9 tokens) without
        /// any casing changes and will be more optimized (perf and relevance)
        /// towards search like queries.&lt;/li&gt;&lt;/ul&gt;
        /// . Possible values include: 'spell', 'proof'
        /// </param>
        /// <param name='preContextText'>
        /// A string that gives context to the text string. For example, the text
        /// string petal is valid; however, if you set preContextText to bike, the
        /// context changes and the text string becomes not valid. In this case, the
        /// API will suggest that you change petal to pedal (as in bike pedal).
        /// </param>
        /// <param name='postContextText'>
        /// A string that gives context to the text string. For example, the text
        /// string read is valid; however, if you set postContextText to carpet, the
        /// context changes and the text string becomes not valid. In this case, the
        /// API will suggest that you change read to red (as in red carpet).
        /// </param>
        /// <param name='mkt'>
        /// For proof mode, only support en-us, es-es, pt-br,
        /// For spell mode, support all language codes. Possible values include:
        /// 'en-us', 'es-es', 'pt-br'
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
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> SpellCheckWithHttpMessagesAsync(string text, string mode = default(string), string preContextText = default(string), string postContextText = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            //string text = "Bill Gatas";
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("mode", mode);
                tracingParameters.Add("text", text);
                tracingParameters.Add("preContextText", preContextText);
                tracingParameters.Add("postContextText", postContextText);
                tracingParameters.Add("mkt", mkt);
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "SpellCheck", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "").ToString();
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
            if (mode != null)
            {
                _queryParameters.Add(string.Format("mode={0}", System.Uri.EscapeDataString(mode)));
            }
            if (text != null)
            {
                _queryParameters.Add(string.Format("text={0}", System.Uri.EscapeDataString(text)));
            }
            if (preContextText != null)
            {
                _queryParameters.Add(string.Format("preContextText={0}", System.Uri.EscapeDataString(preContextText)));
            }
            if (postContextText != null)
            {
                _queryParameters.Add(string.Format("postContextText={0}", System.Uri.EscapeDataString(postContextText)));
            }
            if (mkt != null)
            {
                _queryParameters.Add(string.Format("mkt={0}", System.Uri.EscapeDataString(mkt)));
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
                foreach(var _header in customHeaders)
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
            if ((int)_statusCode != 200 && (int)_statusCode != 400 && (int)_statusCode != 401 && (int)_statusCode != 403 && (int)_statusCode != 429)
            {
                var ex = new Microsoft.Rest.HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null) {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else {
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
