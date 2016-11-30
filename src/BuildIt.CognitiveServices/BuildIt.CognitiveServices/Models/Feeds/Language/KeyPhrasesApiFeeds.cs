using System.Collections.Generic;

namespace BuildIt.CognitiveServices.Models.Feeds.Language
{
    public class KeyPhrasesApiFeeds
    {
        public List<KeyPhrasesDoc> documents { get; set; }
        public List<KeyPhrasesError> errors { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
    public class KeyPhrasesDoc
    {
        public List<string> keyPhrases { get; set; }
        public string id { get; set; }
    }
    public class KeyPhrasesError
    {
        public string id { get; set; }
        public string message { get; set; }
    }

}
