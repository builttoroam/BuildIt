// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class UsageStatisticsResponse
    {
        /// <summary>
        /// Initializes a new instance of the UsageStatisticsResponse class.
        /// </summary>
        public UsageStatisticsResponse() { }

        /// <summary>
        /// Initializes a new instance of the UsageStatisticsResponse class.
        /// </summary>
        /// <param name="interval">The input interval for this response</param>
        /// <param name="buildId">The input buildId (note that this is
        /// optional)</param>
        /// <param name="statistics">The usage statistics</param>
        public UsageStatisticsResponse(string interval = default(string), long? buildId = default(long?), System.Collections.Generic.IList<UsageStatistics> statistics = default(System.Collections.Generic.IList<UsageStatistics>))
        {
            Interval = interval;
            BuildId = buildId;
            Statistics = statistics;
        }

        /// <summary>
        /// Gets or sets the input interval for this response
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "interval")]
        public string Interval { get; set; }

        /// <summary>
        /// Gets or sets the input buildId (note that this is optional)
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "buildId")]
        public long? BuildId { get; set; }

        /// <summary>
        /// Gets or sets the usage statistics
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "statistics")]
        public System.Collections.Generic.IList<UsageStatistics> Statistics { get; set; }

    }
}
