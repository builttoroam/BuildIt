// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{

    /// <summary>
    /// The Video API lets partners send a search query to Bing and get back a
    /// list of relevant videos. Note you should call the Video API if you
    /// need video content only. If you need other content such as news and
    /// webpages in addition to videos, you must call the Search API which
    /// includes videos in the response. You must display the videos in the
    /// order provided in the response.
    /// </summary>
    public partial interface IVideoSearchAPIV5 : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        Newtonsoft.Json.JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        Newtonsoft.Json.JsonSerializerSettings DeserializationSettings { get; }


            /// <summary>
        /// Get videos relevant for a given query.
        /// </summary>
        /// <param name='count'>
        /// The number of video results to return in the response. The actual
        /// number delivered may be less than requested.
        /// </param>
        /// <param name='offset'>
        /// The zero-based offset that indicates the number of video results
        /// to skip before returning results.
        /// </param>
        /// <param name='mkt'>
        /// The market where the results come from. Typically, this is the
        /// country where the user is making the request from; however, it
        /// could be a different country if the user is not located in a
        /// country where Bing delivers results. The market must be in the
        /// form {language code}-{country code}. For example, en-US.
        /// 
        /// &lt;br&gt;
        /// &lt;br&gt;
        /// Full list of supported markets:
        /// &lt;br&gt;
        /// es-AR,en-AU,de-AT,nl-BE,fr-BE,pt-BR,en-CA,fr-CA,es-CL,da-DK,fi-FI,fr-FR,de-DE,zh-HK,en-IN,en-ID,en-IE,it-IT,ja-JP,ko-KR,en-MY,es-MX,nl-NL,en-NZ,no-NO,zh-CN,pl-PL,pt-PT,en-PH,ru-RU,ar-SA,en-ZA,es-ES,sv-SE,fr-CH,de-CH,zh-TW,tr-TR,en-GB,en-US,es-US.
        /// Possible values include: 'en-us'
        /// </param>
        /// <param name='safeSearch'>
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
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> SearchWithHttpMessagesAsync(double? count = 10, double? offset = 0, string mkt = "en-us", string safeSearch = "Moderate", string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Get currently trending videos.
        /// </summary>
        /// <param name='subscriptionKey'>
        /// subscription key in url
        /// </param>
        /// <param name='ocpApimSubscriptionKey'>
        /// subscription key in header
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> TrendingWithHttpMessagesAsync(string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Get currently trending videos.
        /// </summary>
        /// <param name='id'>
        /// An ID that uniquely identifies a video. The Video object's videoId
        /// field contains the ID that you set this parameter to.
        /// </param>
        /// <param name='modulesRequested'>
        /// A comma-delimited list of one or more insights to request.
        /// Possible values include: 'All', 'RelatedVideos', 'VideoResult'
        /// </param>
        /// <param name='subscriptionKey'>
        /// subscription key in url
        /// </param>
        /// <param name='ocpApimSubscriptionKey'>
        /// subscription key in header
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> DetailsWithHttpMessagesAsync(string id = default(string), string modulesRequested = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }
}
