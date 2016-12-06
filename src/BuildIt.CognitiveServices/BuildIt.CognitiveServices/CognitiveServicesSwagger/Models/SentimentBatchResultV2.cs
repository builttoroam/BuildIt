// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class SentimentBatchResultV2
    {
        /// <summary>
        /// Initializes a new instance of the SentimentBatchResultV2 class.
        /// </summary>
        public SentimentBatchResultV2() { }

        /// <summary>
        /// Initializes a new instance of the SentimentBatchResultV2 class.
        /// </summary>
        public SentimentBatchResultV2(System.Collections.Generic.IList<SentimentBatchResultItemV2> documents = default(System.Collections.Generic.IList<SentimentBatchResultItemV2>), System.Collections.Generic.IList<ErrorRecordV2> errors = default(System.Collections.Generic.IList<ErrorRecordV2>))
        {
            Documents = documents;
            Errors = errors;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "documents")]
        public System.Collections.Generic.IList<SentimentBatchResultItemV2> Documents { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "errors")]
        public System.Collections.Generic.IList<ErrorRecordV2> Errors { get; set; }

    }
}
