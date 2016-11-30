using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    class TextAnalyticsResutBody
    {
        public string id { get; set; }
        public string text { get; set; }
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

    public class Error
    {
        public string id { get; set; }
        public string message { get; set; }
    }

    public class TextAnalyticsReplyBody
    {
        public List<Document> documents { get; set; }
        public List<Error> errors { get; set; }
    }

    public class DetectkeyPhrasesDoc
    {
        public List<string> keyPhrases { get; set; }
        public string id { get; set; }
    }

    public class DetectkeyPhrases
    {
        public List<DetectkeyPhrasesDoc> documents { get; set; }
        public List<object> errors { get; set; }
    }

    public class SentimentDoc
    {
        public double score { get; set; }
        public string id { get; set; }
    }

    public class Sentiment
    {
        public List<SentimentDoc> documents { get; set; }
        public List<object> errors { get; set; }
    }

    public class DetectLanguageDoc
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class DetectLanguage
    {
        public List<DetectLanguageDoc> documents { get; set; }
    }
}
