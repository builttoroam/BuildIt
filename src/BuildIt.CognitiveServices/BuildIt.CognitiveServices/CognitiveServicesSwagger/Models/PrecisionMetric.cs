// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class PrecisionMetric
    {
        /// <summary>
        /// Initializes a new instance of the PrecisionMetric class.
        /// </summary>
        public PrecisionMetric() { }

        /// <summary>
        /// Initializes a new instance of the PrecisionMetric class.
        /// </summary>
        /// <param name="k">The value K used to calculate the metric
        /// values.</param>
        /// <param name="percentage">Precision@K percentage.</param>
        /// <param name="usersInTest">The total number of users in the test
        /// dataset.</param>
        /// <param name="usersConsidered">A user is only considered if the
        /// system recommended at least K items based on the model generated
        /// using the training dataset.</param>
        /// <param name="usersNotConsidered">Any users not considered; the
        /// users that did not receive at least K recommended items.</param>
        public PrecisionMetric(int? k = default(int?), double? percentage = default(double?), int? usersInTest = default(int?), int? usersConsidered = default(int?), int? usersNotConsidered = default(int?))
        {
            K = k;
            Percentage = percentage;
            UsersInTest = usersInTest;
            UsersConsidered = usersConsidered;
            UsersNotConsidered = usersNotConsidered;
        }

        /// <summary>
        /// Gets or sets the value K used to calculate the metric values.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "k")]
        public int? K { get; set; }

        /// <summary>
        /// Gets or sets precision@K percentage.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "percentage")]
        public double? Percentage { get; set; }

        /// <summary>
        /// Gets or sets the total number of users in the test dataset.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "usersInTest")]
        public int? UsersInTest { get; set; }

        /// <summary>
        /// Gets or sets a user is only considered if the system recommended
        /// at least K items based on the model generated using the training
        /// dataset.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "usersConsidered")]
        public int? UsersConsidered { get; set; }

        /// <summary>
        /// Gets or sets any users not considered; the users that did not
        /// receive at least K recommended items.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "usersNotConsidered")]
        public int? UsersNotConsidered { get; set; }

    }
}
