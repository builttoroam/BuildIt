// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{
    using Microsoft.Rest;

    public partial class EmotionAPI : Microsoft.Rest.ServiceClient<EmotionAPI>, IEmotionAPI
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
        /// Initializes a new instance of the EmotionAPI class.
        /// </summary>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public EmotionAPI(params System.Net.Http.DelegatingHandler[] handlers) : base(handlers)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the EmotionAPI class.
        /// </summary>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public EmotionAPI(System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : base(rootHandler, handlers)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the EmotionAPI class.
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
        public EmotionAPI(System.Uri baseUri, params System.Net.Http.DelegatingHandler[] handlers) : this(handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            this.BaseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the EmotionAPI class.
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
        public EmotionAPI(System.Uri baseUri, System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : this(rootHandler, handlers)
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
            this.BaseUri = new System.Uri("https://westus.api.cognitive.microsoft.com/emotion/v1.0");
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
        /// <summary>
        /// &lt;p&gt;Recognizes the emotions expressed by one or more people in an
        /// image, as well as returns a bounding box for the face. The emotions
        /// detected are happiness, sadness, surprise, anger, fear, contempt, and
        /// disgust or neutral. &lt;br/&gt;&amp;bull; The supported input image
        /// formats includes JPEG, PNG, GIF(the first frame), BMP. Image file size
        /// should be no larger than 4MB. &lt;br/&gt;&amp;bull; If a user has already
        /// called the Face API, they can submit the face rectangles as an optional
        /// input. Otherwise, Emotion API will first compute the rectangles.
        /// &lt;br/&gt;&amp;bull; The detectable face size range is 36x36 to
        /// 4096x4096 pixels. Faces out of this range will not be detected.
        /// &lt;br/&gt;&amp;bull; For each image, the maximum number of faces
        /// detected is 64 and the faces are ranked by face rectangle size in
        /// descending order. If no face is detected, an empty array will be
        /// returned. &lt;br/&gt;&amp;bull; Some faces may not be detected due to
        /// technical challenges, e.g. very large face angles (head-pose), large
        /// occlusion. Frontal and near-frontal faces have the best results.
        /// &lt;br/&gt;&amp;bull; The emotions contempt and disgust are
        /// experimental.&lt;/p&gt;
        /// </summary>
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
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> EmotionRecognitionWithHttpMessagesAsync(string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "EmotionRecognition", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "recognize").ToString();
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
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

        /// <summary>
        /// &lt;p&gt;Welcome to the Microsoft Emotion API for Video. Emotion API for
        /// Video is a cloud-based API that recognizes the emotions expressed by the
        /// people in an image and returns their emotions. The emotions detected are
        /// happiness, sadness, surprise, anger, fear, contempt, and disgust or
        /// neutral. These APIs allow you to build more personalized and intelligent
        /// apps by understanding your video content. &lt;/p&gt;
        /// &lt;br/&gt;
        /// Emotion Recognition&lt;br/&gt;
        /// Returns aggregate emotions for the faces in a video.&lt;br/&gt;
        /// &amp;bull; The supported input video formats include MP4, MOV, and WMV.
        /// Video file size should be no larger than 100MB. &lt;br/&gt;
        /// &amp;bull; The detectable face size range is 24x24 to 2048x2048 pixels.
        /// The faces out of this range will not be detected. &lt;br/&gt;
        /// &amp;bull; For each video, the maximum number of faces returned is 64.
        /// &lt;br/&gt;
        /// &amp;bull; Some faces may not be detected due to technical challenges;
        /// e.g. very large face angles (head-pose), and large occlusion. Frontal and
        /// near-frontal faces have the best results. &lt;br/&gt;
        /// &amp;bull; Output files are deleted after 24 hours. &lt;br/&gt;
        /// </summary>
        /// <param name='outputStyle'>
        /// Defaults to “aggregate” style, but a user can specify “perFrame” style.
        /// Possible values include: 'aggregate', 'perFrame'
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
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> EmotionRecognitioninVideoWithHttpMessagesAsync(string outputStyle = "aggregate", string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("outputStyle", outputStyle);
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "EmotionRecognitioninVideo", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "recognizeinvideo").ToString();
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
            if (outputStyle != null)
            {
                _queryParameters.Add(string.Format("outputStyle={0}", System.Uri.EscapeDataString(outputStyle)));
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
            if ((int)_statusCode != 202 && (int)_statusCode != 400 && (int)_statusCode != 415 && (int)_statusCode != 401 && (int)_statusCode != 403 && (int)_statusCode != 429)
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

        /// <summary>
        /// Get operation result. If succeeded, this interface returns a JSON that
        /// includes time stamps and operation status/result. Below is an example:
        /// &lt;br/&gt;
        /// 
        /// Example JSON:
        /// &lt;table class="element table"&gt;
        /// &lt;thead&gt;
        /// &lt;/thead&gt;
        /// &lt;tbody&gt;
        /// &lt;tr&gt;
        /// {&lt;br/&gt;
        /// "status": "Running",&lt;br/&gt;
        /// "createdDateTime":  "2015-09-30T01:28:23.4493273Z",&lt;br/&gt;
        /// "lastActionDateTime": "2015-09-30T01:32:23.0895791Z",&lt;br/&gt;
        /// }&lt;br/&gt;
        /// &lt;/tr&gt;
        /// &lt;/tbody&gt;
        /// &lt;/table&gt;
        /// &lt;br/&gt;
        /// &lt;p&gt;
        /// Possible values of "status" field are:&lt;br/&gt;
        /// &lt;b&gt;Not Started&lt;/b&gt; - video content is received/uploaded but
        /// the process has not started.&lt;br/&gt;
        /// &lt;b&gt;Uploading&lt;/b&gt; - the video content is being uploaded by the
        /// URL client side provides.&lt;br/&gt;
        /// &lt;b&gt;Running&lt;/b&gt; - the process is running.&lt;br/&gt;
        /// &lt;b&gt;Failed&lt;/b&gt; - the process is failed. Detailed information
        /// will be provided in "message" field.&lt;br/&gt;
        /// &lt;b&gt;Succeeded&lt;/b&gt; - the process succeeded. In this case,
        /// depending on specific operation client side created, the result can be
        /// retrieved in following two ways:&lt;br/&gt;
        /// &lt;/p&gt;
        /// The result (as a JSON in string) is available in
        /// &lt;b&gt;processingResult&lt;/b&gt; field.
        /// </summary>
        /// <param name='oid'>
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
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> GetRecognitioninVideoOperationResultWithHttpMessagesAsync(string oid, string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (oid == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "oid");
            }
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("oid", oid);
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "GetRecognitioninVideoOperationResult", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "operations/{oid}").ToString();
            _url = _url.Replace("{oid}", System.Uri.EscapeDataString(oid));
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
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
            if ((int)_statusCode != 200 && (int)_statusCode != 400 && (int)_statusCode != 404 && (int)_statusCode != 401 && (int)_statusCode != 403 && (int)_statusCode != 429)
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

        /// <summary>
        /// &lt;p&gt;Recognizes the emotions expressed by one or more people in an
        /// image, as well as returns a bounding box for the face. The emotions
        /// detected are happiness, sadness, surprise, anger, fear, contempt, and
        /// disgust or neutral. &lt;br/&gt;&amp;bull; The supported input image
        /// formats includes JPEG, PNG, GIF(the first frame), BMP. Image file size
        /// should be no larger than 4MB. &lt;br/&gt;&amp;bull; If a user has already
        /// called the Face API, they can submit the face rectangles as an optional
        /// input. Otherwise, Emotion API will first compute the rectangles.
        /// &lt;br/&gt;&amp;bull; The detectable face size range is 36x36 to
        /// 4096x4096 pixels. Faces out of this range will not be detected.
        /// &lt;br/&gt;&amp;bull; For each image, the maximum number of faces
        /// detected is 64 and the faces are ranked by face rectangle size in
        /// descending order. If no face is detected, an empty array will be
        /// returned. &lt;br/&gt;&amp;bull; Some faces may not be detected due to
        /// technical challenges, e.g. very large face angles (head-pose), large
        /// occlusion. Frontal and near-frontal faces have the best results.
        /// &lt;br/&gt;&amp;bull; The emotions contempt and disgust are
        /// experimental.&lt;/p&gt;
        /// </summary>
        /// <param name='faceRectangles'>
        /// A face rectangle is in the form “left,top,width,height”. Delimited
        /// multiple face rectangles with a “;”.
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
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> EmotionRecognitionwithFaceRectanglesWithHttpMessagesAsync(string faceRectangles, string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (faceRectangles == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "faceRectangles");
            }
            // Tracing
            bool _shouldTrace = Microsoft.Rest.ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = Microsoft.Rest.ServiceClientTracing.NextInvocationId.ToString();
                System.Collections.Generic.Dictionary<string, object> tracingParameters = new System.Collections.Generic.Dictionary<string, object>();
                tracingParameters.Add("faceRectangles", faceRectangles);
                tracingParameters.Add("subscriptionKey", subscriptionKey);
                tracingParameters.Add("ocpApimSubscriptionKey", ocpApimSubscriptionKey);
                tracingParameters.Add("cancellationToken", cancellationToken);
                Microsoft.Rest.ServiceClientTracing.Enter(_invocationId, this, "EmotionRecognitionwithFaceRectangles", tracingParameters);
            }
            // Construct URL
            var _baseUrl = this.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "recognize").ToString();
            System.Collections.Generic.List<string> _queryParameters = new System.Collections.Generic.List<string>();
            if (faceRectangles != null)
            {
                _queryParameters.Add(string.Format("faceRectangles={0}", System.Uri.EscapeDataString(faceRectangles)));
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

    }
}
