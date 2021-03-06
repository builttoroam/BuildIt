// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    /// <summary>
    /// Usage event
    /// </summary>
    public partial class UsageEvent
    {
        /// <summary>
        /// Initializes a new instance of the UsageEvent class.
        /// </summary>
        public UsageEvent() { }

        /// <summary>
        /// Initializes a new instance of the UsageEvent class.
        /// </summary>
        /// <param name="userId">The id of the user that created the
        /// events</param>
        /// <param name="buildId">The build id associated with the
        /// events</param>
        /// <param name="events">The events information</param>
        public UsageEvent(string userId = default(string), long? buildId = default(long?), System.Collections.Generic.IList<UsageEventInfo> events = default(System.Collections.Generic.IList<UsageEventInfo>))
        {
            UserId = userId;
            BuildId = buildId;
            Events = events;
        }

        /// <summary>
        /// Gets or sets the id of the user that created the events
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the build id associated with the events
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "buildId")]
        public long? BuildId { get; set; }

        /// <summary>
        /// Gets or sets the events information
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "events")]
        public System.Collections.Generic.IList<UsageEventInfo> Events { get; set; }

    }
}
