// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class FeatureBlockListParameters
    {
        /// <summary>
        /// Initializes a new instance of the FeatureBlockListParameters class.
        /// </summary>
        public FeatureBlockListParameters() { }

        /// <summary>
        /// Initializes a new instance of the FeatureBlockListParameters class.
        /// </summary>
        public FeatureBlockListParameters(string name = default(string), System.Collections.Generic.IList<string> values = default(System.Collections.Generic.IList<string>))
        {
            Name = name;
            Values = values;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "values")]
        public System.Collections.Generic.IList<string> Values { get; set; }

    }
}
