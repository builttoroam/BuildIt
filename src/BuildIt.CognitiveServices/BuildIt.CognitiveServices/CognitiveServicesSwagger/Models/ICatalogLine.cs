// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class ICatalogLine
    {
        /// <summary>
        /// Initializes a new instance of the ICatalogLine class.
        /// </summary>
        public ICatalogLine() { }

        /// <summary>
        /// Initializes a new instance of the ICatalogLine class.
        /// </summary>
        public ICatalogLine(string id = default(string), string name = default(string), string category = default(string), string description = default(string), System.Collections.Generic.IList<ItemFeature> features = default(System.Collections.Generic.IList<ItemFeature>), string metadata = default(string))
        {
            Id = id;
            Name = name;
            Category = category;
            Description = description;
            Features = features;
            Metadata = metadata;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "category")]
        public string Category { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "features")]
        public System.Collections.Generic.IList<ItemFeature> Features { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "metadata")]
        public string Metadata { get; private set; }

    }
}
