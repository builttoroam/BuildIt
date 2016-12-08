// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{

    /// <summary>
    /// </summary>
    public partial interface IAcademicSearchAPI : System.IDisposable
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
        /// The interpret REST API takes an end user query string (i.e., a
        /// query entered by a user of your application) and returns
        /// formatted interpretations of user intent based on the Academic
        /// Graph data and the Academic Grammar.
        /// To provide an interactive experience, you can call this method
        /// repeatedly after each character entered by the user. In that
        /// case, you should set the complete parameter to 1 to enable
        /// auto-complete suggestions. If your application does not want
        /// auto-completion, you should set the complete parameter to 0.
        /// </summary>
        /// <param name='query'>
        /// Query entered by user. If complete is set to 1, query will be
        /// interpreted as a prefix for generating query auto-completion
        /// suggestions.
        /// </param>
        /// <param name='complete'>
        /// 1 means that auto-completion suggestions are generated based on
        /// the grammar and graph data.
        /// </param>
        /// <param name='count'>
        /// Maximum number of interpretations to return.
        /// </param>
        /// <param name='offset'>
        /// Index of the first interpretation to return. For example,
        /// count=2&amp;offset=0 returns interpretations 0 and 1.
        /// count=2&amp;offset=2 returns interpretations 2 and 3.
        /// </param>
        /// <param name='timeout'>
        /// Timeout in milliseconds. Only interpretations found before the
        /// timeout has elapsed are returned.
        /// </param>
        /// <param name='model'>
        /// Name of the model that you wish to query. Currently, the value
        /// defaults to "latest".
        /// . Possible values include: 'beta-2015', 'latest'
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> InterpretWithHttpMessagesAsync(string query, int? complete = 0, int? count = 10, int? offset = default(int?), int? timeout = default(int?), string model = "latest", string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// The evaluate REST API is used to return a set of academic entities
        /// based on a query expression.
        /// </summary>
        /// <param name='expr'>
        /// A query expression that specifies which entities should be
        /// returned.
        /// </param>
        /// <param name='model'>
        /// Name of the model that you wish to query. Currently, the value
        /// defaults to "latest".
        /// . Possible values include: 'beta-2015', 'latest'
        /// </param>
        /// <param name='count'>
        /// Number of results to return.
        /// </param>
        /// <param name='offset'>
        /// Index of the first result to return.
        /// </param>
        /// <param name='orderby'>
        /// Name of an attribute that is used for sorting the entities.
        /// Optionally, ascending/descending can be specified. The format is:
        /// name:asc or name:desc.
        /// </param>
        /// <param name='attributes'>
        /// A comma delimited list that specifies the attribute values that
        /// are included in the response. Attribute names are case-sensitive.
        /// Possible values include: 'Id'
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> EvaluateWithHttpMessagesAsync(string expr, string model = "latest", int? count = 10, int? offset = 0, string orderby = default(string), string attributes = "Id", string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// The calchistogram REST API is used to calculate the distribution
        /// of attribute values for a set of paper entities.
        /// </summary>
        /// <param name='expr'>
        /// A query expression that specifies the entities over which to
        /// calculate histograms.
        /// </param>
        /// <param name='model'>
        /// Name of the model that you wish to query. Currently, the value
        /// defaults to "latest".
        /// . Possible values include: 'beta-2015', 'latest'
        /// </param>
        /// <param name='attributes'>
        /// A comma delimited list that specifies the attribute values that
        /// are included in the response. Attribute names are case-sensitive.
        /// </param>
        /// <param name='count'>
        /// Number of results to return.
        /// </param>
        /// <param name='offset'>
        /// Index of the first result to return.
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> CalcHistogramWithHttpMessagesAsync(string expr, string model = "latest", string attributes = default(string), int? count = 10, int? offset = 0, string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Welcome to the Microsoft Cognitive Service Academic Search API, a
        /// web service for retrieving paths and subgraphs from Microsoft
        /// Academic Graph. The graph query interface powered by Graph Engine
        /// allows us to not only query entities that meet certain criteria
        /// (e.g. find a paper with a given title), but also perform pattern
        /// matching via graph exploration (e.g. detect co-authorship).
        /// </summary>
        /// <param name='mode'>
        /// Request type of query. Should be "json" or "lambda". Possible
        /// values include: 'json', 'lambda'
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
        System.Threading.Tasks.Task<Microsoft.Rest.HttpOperationResponse> GraphSearchWithHttpMessagesAsync(string mode, string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }
}
