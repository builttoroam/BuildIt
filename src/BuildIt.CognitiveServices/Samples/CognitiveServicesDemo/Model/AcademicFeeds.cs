using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    public class AcademicFeeds
    {
        public string query { get; set; }
        public List<Interpretation> interpretations { get; set; }
        public AcademicError error { get; set; }
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
    public class AcademicError
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
