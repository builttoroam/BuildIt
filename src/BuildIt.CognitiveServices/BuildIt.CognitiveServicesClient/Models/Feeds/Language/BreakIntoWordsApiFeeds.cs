using System.Collections.Generic;

namespace BuildIt.CognitiveServicesClient.Models.Feeds.Language
{
    public class BreakIntoWordsApiFeeds
    {
        public List<Candidate> candidates { get; set; }
        public BreakIntoWordsError error { get; set; }

    }
    public class Candidate
    {
        public string words { get; set; }
        public double probability { get; set; }
    }

    public class BreakIntoWordsError
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
