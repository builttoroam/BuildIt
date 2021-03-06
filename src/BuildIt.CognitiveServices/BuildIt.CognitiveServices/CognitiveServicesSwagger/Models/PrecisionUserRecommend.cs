// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class PrecisionUserRecommend
    {
        /// <summary>
        /// Initializes a new instance of the PrecisionUserRecommend class.
        /// </summary>
        public PrecisionUserRecommend() { }

        /// <summary>
        /// Initializes a new instance of the PrecisionUserRecommend class.
        /// </summary>
        /// <param name="precisionMetrics">Precision metrics that are computed
        /// for the test/train dataset.</param>
        /// <param name="error">Error message to indicate reason in failure
        /// cases.</param>
        public PrecisionUserRecommend(System.Collections.Generic.IList<PrecisionMetric> precisionMetrics = default(System.Collections.Generic.IList<PrecisionMetric>), string error = default(string))
        {
            PrecisionMetrics = precisionMetrics;
            Error = error;
        }

        /// <summary>
        /// Gets or sets precision metrics that are computed for the
        /// test/train dataset.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "precisionMetrics")]
        public System.Collections.Generic.IList<PrecisionMetric> PrecisionMetrics { get; set; }

        /// <summary>
        /// Gets or sets error message to indicate reason in failure cases.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

    }
}
