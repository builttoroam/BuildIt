// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class UpsaleParameters
    {
        /// <summary>
        /// Initializes a new instance of the UpsaleParameters class.
        /// </summary>
        public UpsaleParameters() { }

        /// <summary>
        /// Initializes a new instance of the UpsaleParameters class.
        /// </summary>
        public UpsaleParameters(System.Collections.Generic.IList<string> itemIds = default(System.Collections.Generic.IList<string>), int? numberOfItemsToUpsale = default(int?))
        {
            ItemIds = itemIds;
            NumberOfItemsToUpsale = numberOfItemsToUpsale;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "itemIds")]
        public System.Collections.Generic.IList<string> ItemIds { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfItemsToUpsale")]
        public int? NumberOfItemsToUpsale { get; set; }

    }
}
