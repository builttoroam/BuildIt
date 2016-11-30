using BuildIt.CognitiveServices.Common;

namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class BreakIntoWordsParameters
    {
        public string subscriptionKey { get; set; }
        public string text { get; set; }
        public int maxNumOfCandidatesReturned { get; set; } = 5;
        public int order { get; set; } = 5;
        public string model { get; set; } = "title";
        public string content { get; set; } = Constants.DefaultContentType;
    }
}
