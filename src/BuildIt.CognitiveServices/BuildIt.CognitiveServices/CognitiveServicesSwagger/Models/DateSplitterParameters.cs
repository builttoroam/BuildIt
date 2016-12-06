// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class DateSplitterParameters
    {
        /// <summary>
        /// Initializes a new instance of the DateSplitterParameters class.
        /// </summary>
        public DateSplitterParameters() { }

        /// <summary>
        /// Initializes a new instance of the DateSplitterParameters class.
        /// </summary>
        /// <param name="splitDate">The split date at which the usage file
        /// data is put in the test set
        /// during splitting.</param>
        public DateSplitterParameters(System.DateTime? splitDate = default(System.DateTime?))
        {
            SplitDate = splitDate;
        }

        /// <summary>
        /// Gets or sets the split date at which the usage file data is put in
        /// the test set
        /// during splitting.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "splitDate")]
        public System.DateTime? SplitDate { get; set; }

    }
}
