// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{

    /// <summary>
    /// </summary>
    public partial interface ISpellCheckAPIV5 : System.IDisposable
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


        /// <param name='mode'>
        /// Mode of spellcheck:
        /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like
        /// spelling corrections. It can correct long queries, provide casing
        /// corrections and suppresses aggressive corrections.&lt;/li&gt;
        /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
        /// corrections. It will correct small queries(up to length 9 tokens)
        /// without any casing changes and will be more optimized (perf and
        /// relevance) towards search like queries.&lt;/li&gt;&lt;/ul&gt;
        /// . Possible values include: 'spell', 'proof'
        /// </param>
        /// <param name='mkt'>
        /// For proof mode, only support en-us, es-es, pt-br,
        /// For spell mode, support all language codes. Possible values
        /// include: 'en-us', 'es-es', 'pt-br'
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> SpellCheckWithHttpMessagesAsync(string mode = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <param name='mode'>
        /// Mode of spellcheck:
        /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like
        /// spelling corrections. It can correct long queries, provide casing
        /// corrections and suppresses aggressive corrections.&lt;/li&gt;
        /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
        /// corrections. It will correct small queries(up to length 9 tokens)
        /// without any casing changes and will be more optimized (perf and
        /// relevance) towards search like queries.&lt;/li&gt;&lt;/ul&gt;
        /// . Possible values include: 'spell', 'proof'
        /// </param>
        /// <param name='preContextText'>
        /// A string that gives context to the text string. For example, the
        /// text string petal is valid; however, if you set preContextText to
        /// bike, the context changes and the text string becomes not valid.
        /// In this case, the API will suggest that you change petal to pedal
        /// (as in bike pedal).
        /// </param>
        /// <param name='postContextText'>
        /// A string that gives context to the text string. For example, the
        /// text string read is valid; however, if you set postContextText to
        /// carpet, the context changes and the text string becomes not
        /// valid. In this case, the API will suggest that you change read to
        /// red (as in red carpet).
        /// </param>
        /// <param name='mkt'>
        /// For proof mode, only support en-us, es-es, pt-br,
        /// For spell mode, support all language codes. Possible values
        /// include: 'en-us', 'es-es', 'pt-br'
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> SpellCheckWithHttpMessagesAsync(string text, string mode = default(string), string preContextText = default(string), string postContextText = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }
}
