// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    /// <summary>
    /// A data object representing a paginated list of catalog items, used as
    /// the return type in API4's catalog API
    /// </summary>
    public partial class CatalogItemsPage
    {
        /// <summary>
        /// Initializes a new instance of the CatalogItemsPage class.
        /// </summary>
        public CatalogItemsPage() { }

        /// <summary>
        /// Initializes a new instance of the CatalogItemsPage class.
        /// </summary>
        /// <param name="value">Gets or sets the catalog items in this
        /// page</param>
        /// <param name="nextLink">Gets or sets a link to the next page of
        /// catalog items, if available</param>
        public CatalogItemsPage(System.Collections.Generic.IList<ICatalogLine> value = default(System.Collections.Generic.IList<ICatalogLine>), string nextLink = default(string))
        {
            Value = value;
            NextLink = nextLink;
        }

        /// <summary>
        /// Gets or sets the catalog items in this page
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
        public System.Collections.Generic.IList<ICatalogLine> Value { get; set; }

        /// <summary>
        /// Gets or sets a link to the next page of catalog items, if available
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "@nextLink")]
        public string NextLink { get; set; }

    }
}
