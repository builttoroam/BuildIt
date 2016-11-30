using System.Collections.Generic;

namespace BuildIt.CognitiveServices.Models.Feeds.Academic
{
    public class InterpretApiFeeds
    {
        public string query { get; set; }
        public List<Interpretation> interpretations { get; set; }
        public InterpretError error { get; set; }
    }
    public class InterpretError
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Output
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Rule
    {
        public string name { get; set; }
        public Output output { get; set; }
    }

    public class Interpretation
    {
        public double logprob { get; set; }
        public string parse { get; set; }
        public List<Rule> rules { get; set; }
    }
}
