// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    /// <summary>
    /// Base class for catalog and usage data ingestion import statistics.
    /// </summary>
    public partial class CatalogUpdateStats
    {
        /// <summary>
        /// Initializes a new instance of the CatalogUpdateStats class.
        /// </summary>
        public CatalogUpdateStats() { }

        /// <summary>
        /// Initializes a new instance of the CatalogUpdateStats class.
        /// </summary>
        /// <param name="processedLineCount">Number of total processed
        /// lines</param>
        /// <param name="addedItemCount">Number of new items successfully
        /// added to the catalog</param>
        /// <param name="updatedItemCount">Number of successfully updated
        /// items</param>
        /// <param name="errorLineCount">Number of total error lines</param>
        /// <param name="errorSummary">Details of errors during catalog
        /// update</param>
        /// <param name="sampleErrorDetails">Sample lines having errors during
        /// catalog update</param>
        public CatalogUpdateStats(int? processedLineCount = default(int?), int? addedItemCount = default(int?), int? updatedItemCount = default(int?), int? errorLineCount = default(int?), System.Collections.Generic.IList<ErrorStats> errorSummary = default(System.Collections.Generic.IList<ErrorStats>), System.Collections.Generic.IList<ErrorDetail> sampleErrorDetails = default(System.Collections.Generic.IList<ErrorDetail>))
        {
            ProcessedLineCount = processedLineCount;
            AddedItemCount = addedItemCount;
            UpdatedItemCount = updatedItemCount;
            ErrorLineCount = errorLineCount;
            ErrorSummary = errorSummary;
            SampleErrorDetails = sampleErrorDetails;
        }

        /// <summary>
        /// Gets or sets number of total processed lines
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "processedLineCount")]
        public int? ProcessedLineCount { get; set; }

        /// <summary>
        /// Gets or sets number of new items successfully added to the catalog
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "addedItemCount")]
        public int? AddedItemCount { get; set; }

        /// <summary>
        /// Gets or sets number of successfully updated items
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "updatedItemCount")]
        public int? UpdatedItemCount { get; set; }

        /// <summary>
        /// Gets or sets number of total error lines
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "errorLineCount")]
        public int? ErrorLineCount { get; set; }

        /// <summary>
        /// Gets or sets details of errors during catalog update
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "errorSummary")]
        public System.Collections.Generic.IList<ErrorStats> ErrorSummary { get; set; }

        /// <summary>
        /// Gets or sets sample lines having errors during catalog update
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "sampleErrorDetails")]
        public System.Collections.Generic.IList<ErrorDetail> SampleErrorDetails { get; set; }

    }
}