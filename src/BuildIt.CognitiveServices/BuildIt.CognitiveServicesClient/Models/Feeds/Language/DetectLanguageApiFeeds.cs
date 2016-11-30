using System.Collections.Generic;

namespace BuildIt.CognitiveServicesClient.Models.Feeds.Language
{
    public class DetectLanguageApiFeeds
    {
        public List<Document> documents { get; set; }
        public List<DetectedLanguageError> errors { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
    public class DetectedLanguage
    {
        public string name { get; set; }
        public string iso6391Name { get; set; }
        public double score { get; set; }
    }

    public class Document
    {
        public string id { get; set; }
        public List<DetectedLanguage> detectedLanguages { get; set; }
    }

    public class DetectedLanguageError
    {
        public string id { get; set; }
        public string message { get; set; }
    }
}
