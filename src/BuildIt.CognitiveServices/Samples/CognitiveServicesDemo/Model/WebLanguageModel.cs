using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    public class WebLanguageModel
    {
        public List<Candidate> candidates { get; set; }
        public WLMError error { get; set; }
    }
    public class Candidate
    {
        public string words { get; set; }
        public double probability { get; set; }
    }

    public class WLMError
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
