// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    /// <summary>
    /// response object for GetAllRules API
    /// </summary>
    public partial class RuleInfoList
    {
        /// <summary>
        /// Initializes a new instance of the RuleInfoList class.
        /// </summary>
        public RuleInfoList() { }

        /// <summary>
        /// Initializes a new instance of the RuleInfoList class.
        /// </summary>
        /// <param name="rules">List of Rules for a given user</param>
        public RuleInfoList(System.Collections.Generic.IList<RuleInfo> rules = default(System.Collections.Generic.IList<RuleInfo>))
        {
            Rules = rules;
        }

        /// <summary>
        /// Gets or sets list of Rules for a given user
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "rules")]
        public System.Collections.Generic.IList<RuleInfo> Rules { get; set; }

    }
}