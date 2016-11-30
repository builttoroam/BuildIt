using System.Collections.Generic;

namespace BuildIt.CognitiveServices.Models.Feeds.Language
{
    public class SentimentApiFeeds
    {
        public List<SentimentDoc> documents { get; set; }
        public List<SentimentError> errors { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }

    public class SentimentError
    {
        public string id { get; set; }
        public string message { get; set; }
    }

    public class SentimentDoc
    {
        public double score { get; set; }
        public string id { get; set; }
    }
}