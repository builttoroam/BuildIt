// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{

    /// <summary>
    /// You use the API to scan your content as it is generated. Content
    /// Moderator then processes your content and sends the results along
    /// with relevant information either back to your systems or to the
    /// built-in review tool. You can use this information to take decisions
    /// e.g. take it down, send to human judge, etc.
    /// 
    /// When using the API, images need to have a minimum of 128 pixels and a
    /// maximum file size of 4MB.
    /// Text can be at most 1024 characters long.
    /// If the content passed to the text API or the image API exceeds the
    /// size limits, the API will return an error code that informs about the
    /// issue.
    /// </summary>
    public partial interface IContentModeratorModerate : System.IDisposable
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


            /// <param name='cacheImage'>
        /// Whether to retain the submitted image for future use; defaults to
        /// false if omitted
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> ImageFindFacesWithHttpMessagesAsync(bool? cacheImage = default(bool?), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Returns any text found in the image for the language specified. If
        /// no language is specified in input then the detection defaults to
        /// English.
        /// </summary>
        /// <param name='cacheImage'>
        /// Whether to retain the submitted image for future use; defaults to
        /// false if omitted
        /// </param>
        /// <param name='enhanced'>
        /// When set to True, the image goes through additional processing to
        /// come with additional candidates.
        /// 
        /// image/tiff is not supported when enhanced is set to true
        /// 
        /// Note: This impacts the response time.
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> ImageOCRWithHttpMessagesAsync(bool? cacheImage = false, bool? enhanced = false, string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Returns probabilities of the image containing racy or adult
        /// content.
        /// </summary>
        /// <param name='cacheImage'>
        /// Whether to retain the submitted image for future use; defaults to
        /// false if omitted
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> ImageEvaluateWithHttpMessagesAsync(bool? cacheImage = default(bool?), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Fuzzily match an image against one of your custom Image Lists. You
        /// can create and manage your custom image lists using &lt;a
        /// href="/docs/services/578ff44d2703741568569ab9/operations/578ff7b12703741568569abe"&gt;this&lt;/a&gt;
        /// API.
        /// 
        /// Returns ID and tags of matching image.&lt;br/&gt;
        /// &lt;br/&gt;
        /// Note: Refresh Index must be run on the corresponding Image List
        /// before additions and removals are reflected in the response.
        /// </summary>
        /// <param name='listId'>
        /// </param>
        /// <param name='cacheimage'>
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> ImageMatchWithHttpMessagesAsync(string listId = default(string), bool? cacheimage = default(bool?), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// This operation will detect the language of given input content.
        /// Returns the &lt;a
        /// href="http://www-01.sil.org/iso639-3/codes.asp"&gt;ISO 639-3
        /// code&lt;/a&gt; for the predominant language comprising the
        /// submitted text. Over 110 languages supported.
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> TextDetectLanguageWithHttpMessagesAsync(string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// The operation detects profanity in more than 100 languages, report
        /// on suspicious malware and phishing URLs, and match against custom
        /// and shared blacklists.
        /// </summary>
        /// <param name='autocorrect'>
        /// Runs auto correction on the input, before running other operations.
        /// </param>
        /// <param name='urls'>
        /// Detects URLs in the input and analyses each URL to return a score
        /// for Malware, Phishing, and Adult.
        /// </param>
        /// <param name='pII'>
        /// Detects Personal Identifiable Information (PII) in the input.
        /// </param>
        /// <param name='listId'>
        /// The Term list to be for matching
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> TextScreenWithHttpMessagesAsync(bool? autocorrect = default(bool?), bool? urls = default(bool?), bool? pII = default(bool?), string listId = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }
}
