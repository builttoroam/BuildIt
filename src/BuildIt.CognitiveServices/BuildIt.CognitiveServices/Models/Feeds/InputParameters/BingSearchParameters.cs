namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    /// <summary>
    /// Parameters for Bing Search
    /// </summary>
    public class BingSearchParameters
    {
        /// <summary>
        /// Subscription key for bing search
        /// </summary>
        public string subscriptionKey { get; set; }
        /// <summary>
        /// The user's search query string
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// The number of search results to return in the response. The actual number delivered may be less than requested. Default value is 10
        /// </summary>
        public int count { get; set; } = 10;
        /// <summary>
        /// The zero-based offset that indicates the number of search results to skip before returning results. Default value is 0
        /// </summary>
        public int offset { get; set; } = 0;
        /// <summary>
        /// The market where the results come from. Default value is en-us
        /// </summary>
        public string mkt { get; set; } = "en-us";
        /// <summary>
        /// A filter used to filter results for adult content. Default value is Moderate
        /// </summary>
        public string safesearch { get; set; } = "Moderate";
    }
}