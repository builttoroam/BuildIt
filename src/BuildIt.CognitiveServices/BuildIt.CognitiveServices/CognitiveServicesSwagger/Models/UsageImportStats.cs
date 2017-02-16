// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class UsageImportStats
    {
        /// <summary>
        /// Initializes a new instance of the UsageImportStats class.
        /// </summary>
        public UsageImportStats() { }

        /// <summary>
        /// Initializes a new instance of the UsageImportStats class.
        /// </summary>
        /// <param name="fileId">Unique identifier for usage file</param>
        /// <param name="processedLineCount">Number of total processed lines
        /// in uploaded catalog file</param>
        /// <param name="errorLineCount">Number of total error lines in
        /// uploaded catalog file</param>
        /// <param name="importedLineCount">Number of successfully imported
        /// catalog items from uploaded catalog file</param>
        /// <param name="errorSummary">Details of errors during catalog
        /// import</param>
        /// <param name="sampleErrorDetails">Sample lines having errors during
        /// catalog import</param>
        public UsageImportStats(string fileId = default(string), int? processedLineCount = default(int?), int? errorLineCount = default(int?), int? importedLineCount = default(int?), System.Collections.Generic.IList<ErrorStats> errorSummary = default(System.Collections.Generic.IList<ErrorStats>), System.Collections.Generic.IList<ErrorDetail> sampleErrorDetails = default(System.Collections.Generic.IList<ErrorDetail>))
        {
            FileId = fileId;
            ProcessedLineCount = processedLineCount;
            ErrorLineCount = errorLineCount;
            ImportedLineCount = importedLineCount;
            ErrorSummary = errorSummary;
            SampleErrorDetails = sampleErrorDetails;
        }

        /// <summary>
        /// Gets or sets unique identifier for usage file
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "fileId")]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets number of total processed lines in uploaded catalog
        /// file
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "processedLineCount")]
        public int? ProcessedLineCount { get; set; }

        /// <summary>
        /// Gets or sets number of total error lines in uploaded catalog file
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "errorLineCount")]
        public int? ErrorLineCount { get; set; }

        /// <summary>
        /// Gets or sets number of successfully imported catalog items from
        /// uploaded catalog file
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "importedLineCount")]
        public int? ImportedLineCount { get; set; }

        /// <summary>
        /// Gets or sets details of errors during catalog import
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "errorSummary")]
        public System.Collections.Generic.IList<ErrorStats> ErrorSummary { get; set; }

        /// <summary>
        /// Gets or sets sample lines having errors during catalog import
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "sampleErrorDetails")]
        public System.Collections.Generic.IList<ErrorDetail> SampleErrorDetails { get; set; }

    }
}
