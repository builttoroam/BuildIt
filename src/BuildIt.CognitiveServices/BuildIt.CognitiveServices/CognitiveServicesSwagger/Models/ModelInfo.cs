// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class ModelInfo
    {
        /// <summary>
        /// Initializes a new instance of the ModelInfo class.
        /// </summary>
        public ModelInfo() { }

        /// <summary>
        /// Initializes a new instance of the ModelInfo class.
        /// </summary>
        /// <param name="id">Unique identifier for Model</param>
        /// <param name="name">Model name, limit 20 characters</param>
        /// <param name="description">Optional model description</param>
        /// <param name="createdDateTime">Model creation date time</param>
        /// <param name="activeBuildId">Active build ID for this model</param>
        /// <param name="catalogDisplayName">Active build ID for this
        /// model</param>
        public ModelInfo(string id = default(string), string name = default(string), string description = default(string), string createdDateTime = default(string), long? activeBuildId = default(long?), string catalogDisplayName = default(string))
        {
            Id = id;
            Name = name;
            Description = description;
            CreatedDateTime = createdDateTime;
            ActiveBuildId = activeBuildId;
            CatalogDisplayName = catalogDisplayName;
        }

        /// <summary>
        /// Gets or sets unique identifier for Model
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets model name, limit 20 characters
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets optional model description
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets model creation date time
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "createdDateTime")]
        public string CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets active build ID for this model
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "activeBuildId")]
        public long? ActiveBuildId { get; set; }

        /// <summary>
        /// Gets or sets active build ID for this model
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "catalogDisplayName")]
        public string CatalogDisplayName { get; set; }

    }
}
