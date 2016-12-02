namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class BingSearchParameters
    {
        public string subscriptionKey { get; set; }
        /// <summary>
        /// The user's search query string
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// The number of search results to return in the response. The actual number delivered may be less than requested.
        /// </summary>
        public int count { get; set; } = 10;
        /// <summary>
        /// The zero-based offset that indicates the number of search results to skip before returning results.
        /// </summary>
        public int offset { get; set; } = 0;
        /// <summary>
        /// The market where the results come from.
        /// </summary>
        public string mkt { get; set; } = "en-us";
        /// <summary>
        /// A filter used to filter results for adult content.
        /// </summary>
        public string safesearch { get; set; } = "Moderate";
    }
}