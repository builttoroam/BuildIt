using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    public class BingSpellCheckFeed
    {
        public string _type { get; set; }
        public List<FlaggedToken> flaggedTokens { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
    public class Suggestion
    {
        public string suggestion { get; set; }
        public int score { get; set; }
    }

    public class FlaggedToken
    {
        public int offset { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public List<Suggestion> suggestions { get; set; }
    }
}
