// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    /// <summary>
    /// This is object returned for the clustering call
    /// </summary>
    public partial class TopicDetectionResultV2 : OperationProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the TopicDetectionResultV2 class.
        /// </summary>
        public TopicDetectionResultV2() { }

        /// <summary>
        /// Initializes a new instance of the TopicDetectionResultV2 class.
        /// </summary>
        public TopicDetectionResultV2(System.Collections.Generic.IList<ErrorRecordV2> errors = default(System.Collections.Generic.IList<ErrorRecordV2>), System.Collections.Generic.IList<TopicInfoRecordV2> topics = default(System.Collections.Generic.IList<TopicInfoRecordV2>), System.Collections.Generic.IList<TopicAssignmentRecordV2> topicAssignments = default(System.Collections.Generic.IList<TopicAssignmentRecordV2>))
            : base(errors)
        {
            Topics = topics;
            TopicAssignments = topicAssignments;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "topics")]
        public System.Collections.Generic.IList<TopicInfoRecordV2> Topics { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "topicAssignments")]
        public System.Collections.Generic.IList<TopicAssignmentRecordV2> TopicAssignments { get; set; }

    }
}
