using BuildIt.CognitiveServices.Common;

namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class BreakIntoWordsParameters
    {
        /// <summary>
        /// Subscription key
        /// </summary>
        public string subscriptionKey { get; set; }
        /// <summary>
        /// The line of text to break into words. If spaces are present, they will be interpreted as hard breaks and maintained, except for leading or trailing spaces, which will be trimmed.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The order of N-gram. If not specified, use default value 5 .Supported value: 1, 2, 3, 4, 5.
        /// </summary>
        public int maxNumOfCandidatesReturned { get; set; } = 5;
        public int order { get; set; } = 5;
        /// <summary>
        /// Which model to use, supported value: title/anchor/query/body. Default is title
        /// </summary>
        public string model { get; set; } = "title";
        /// <summary>
        /// Media type of the body sent to the API. Default is application/json
        /// </summary>
        public string contentType { get; set; } = Constants.DefaultContentType;
    }
}