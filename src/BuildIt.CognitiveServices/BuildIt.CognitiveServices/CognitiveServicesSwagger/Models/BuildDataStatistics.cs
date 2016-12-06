// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class BuildDataStatistics
    {
        /// <summary>
        /// Initializes a new instance of the BuildDataStatistics class.
        /// </summary>
        public BuildDataStatistics() { }

        /// <summary>
        /// Initializes a new instance of the BuildDataStatistics class.
        /// </summary>
        /// <param name="numberOfCatalogItems">Number of items in the
        /// catalog.</param>
        /// <param name="numberOfCatalogItemsInUsage">Number of unique items
        /// from usage data (which are present in catalog).</param>
        /// <param name="numberOfUsers">Number of unique users in usage data
        /// before any pruning.</param>
        /// <param name="numberOfUsageRecords">Total number of usage points
        /// before any pruning, and after removing duplicate user id / item
        /// id records.</param>
        /// <param name="catalogCoverage">NumberOfCatalogItemsForModeling/
        /// NumberOfCatalogItems
        /// This property indicates what part of the catalog can
        /// be modelled with usage data. The rest of the items will need
        /// content-based features.</param>
        /// <param name="numberOfCatalogItemsInBuild">Number of unique items
        /// from usage data that are used for training (which are present in
        /// catalog).</param>
        /// <param name="numberOfUsersInBuild">Number of unique users in usage
        /// data that are used for training.</param>
        /// <param name="numberOfUsageRecordsInBuild">Total number of usage
        /// points that are used for training</param>
        /// <param name="catalogCoverageInBuild">NumberOfCatalogItemsInBuild/
        /// NumberOfCatalogItems
        /// This property indicates what part of the catalog will
        /// be modeled with usage data. The rest of the items will need
        /// content-based features.</param>
        public BuildDataStatistics(int? numberOfCatalogItems = default(int?), int? numberOfCatalogItemsInUsage = default(int?), int? numberOfUsers = default(int?), int? numberOfUsageRecords = default(int?), double? catalogCoverage = default(double?), int? numberOfCatalogItemsInBuild = default(int?), int? numberOfUsersInBuild = default(int?), int? numberOfUsageRecordsInBuild = default(int?), double? catalogCoverageInBuild = default(double?))
        {
            NumberOfCatalogItems = numberOfCatalogItems;
            NumberOfCatalogItemsInUsage = numberOfCatalogItemsInUsage;
            NumberOfUsers = numberOfUsers;
            NumberOfUsageRecords = numberOfUsageRecords;
            CatalogCoverage = catalogCoverage;
            NumberOfCatalogItemsInBuild = numberOfCatalogItemsInBuild;
            NumberOfUsersInBuild = numberOfUsersInBuild;
            NumberOfUsageRecordsInBuild = numberOfUsageRecordsInBuild;
            CatalogCoverageInBuild = catalogCoverageInBuild;
        }

        /// <summary>
        /// Gets or sets number of items in the catalog.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfCatalogItems")]
        public int? NumberOfCatalogItems { get; set; }

        /// <summary>
        /// Gets or sets number of unique items from usage data (which are
        /// present in catalog).
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfCatalogItemsInUsage")]
        public int? NumberOfCatalogItemsInUsage { get; set; }

        /// <summary>
        /// Gets or sets number of unique users in usage data before any
        /// pruning.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfUsers")]
        public int? NumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets total number of usage points before any pruning, and
        /// after removing duplicate user id / item id records.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfUsageRecords")]
        public int? NumberOfUsageRecords { get; set; }

        /// <summary>
        /// Gets or sets numberOfCatalogItemsForModeling/ NumberOfCatalogItems
        /// This property indicates what part of the catalog can
        /// be modelled with usage data. The rest of the items will need
        /// content-based features.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "catalogCoverage")]
        public double? CatalogCoverage { get; set; }

        /// <summary>
        /// Gets or sets number of unique items from usage data that are used
        /// for training (which are present in catalog).
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfCatalogItemsInBuild")]
        public int? NumberOfCatalogItemsInBuild { get; set; }

        /// <summary>
        /// Gets or sets number of unique users in usage data that are used
        /// for training.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfUsersInBuild")]
        public int? NumberOfUsersInBuild { get; set; }

        /// <summary>
        /// Gets or sets total number of usage points that are used for
        /// training
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "numberOfUsageRecordsInBuild")]
        public int? NumberOfUsageRecordsInBuild { get; set; }

        /// <summary>
        /// Gets or sets numberOfCatalogItemsInBuild/ NumberOfCatalogItems
        /// This property indicates what part of the catalog will
        /// be modeled with usage data. The rest of the items will need
        /// content-based features.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "catalogCoverageInBuild")]
        public double? CatalogCoverageInBuild { get; set; }

    }
}
