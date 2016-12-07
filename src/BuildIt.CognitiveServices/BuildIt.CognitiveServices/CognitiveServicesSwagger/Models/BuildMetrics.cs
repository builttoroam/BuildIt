// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class BuildMetrics
    {
        /// <summary>
        /// Initializes a new instance of the BuildMetrics class.
        /// </summary>
        public BuildMetrics() { }

        /// <summary>
        /// Initializes a new instance of the BuildMetrics class.
        /// </summary>
        /// <param name="precisionItemRecommend">Precision metrics for the
        /// build based on single item recommendations.</param>
        /// <param name="precisionUserRecommend">Precision metrics for the
        /// build based on user's history recommendations.</param>
        /// <param name="precisionPopularItemRecommend">Precision metrics for
        /// the build based on popular items recommendations.</param>
        /// <param name="diversityItemRecommend">Diversity metrics for the
        /// build based on single item recommendations.</param>
        /// <param name="diversityUserRecommend">Diversity metrics for the
        /// build based on user's history recommendations.</param>
        public BuildMetrics(PrecisionItemRecommend precisionItemRecommend = default(PrecisionItemRecommend), PrecisionUserRecommend precisionUserRecommend = default(PrecisionUserRecommend), PrecisionPopularItemRecommend precisionPopularItemRecommend = default(PrecisionPopularItemRecommend), DiversityItemRecommend diversityItemRecommend = default(DiversityItemRecommend), DiversityUserRecommend diversityUserRecommend = default(DiversityUserRecommend))
        {
            PrecisionItemRecommend = precisionItemRecommend;
            PrecisionUserRecommend = precisionUserRecommend;
            PrecisionPopularItemRecommend = precisionPopularItemRecommend;
            DiversityItemRecommend = diversityItemRecommend;
            DiversityUserRecommend = diversityUserRecommend;
        }

        /// <summary>
        /// Gets or sets precision metrics for the build based on single item
        /// recommendations.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "precisionItemRecommend")]
        public PrecisionItemRecommend PrecisionItemRecommend { get; set; }

        /// <summary>
        /// Gets or sets precision metrics for the build based on user's
        /// history recommendations.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "precisionUserRecommend")]
        public PrecisionUserRecommend PrecisionUserRecommend { get; set; }

        /// <summary>
        /// Gets or sets precision metrics for the build based on popular
        /// items recommendations.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "precisionPopularItemRecommend")]
        public PrecisionPopularItemRecommend PrecisionPopularItemRecommend { get; set; }

        /// <summary>
        /// Gets or sets diversity metrics for the build based on single item
        /// recommendations.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "diversityItemRecommend")]
        public DiversityItemRecommend DiversityItemRecommend { get; set; }

        /// <summary>
        /// Gets or sets diversity metrics for the build based on user's
        /// history recommendations.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "diversityUserRecommend")]
        public DiversityUserRecommend DiversityUserRecommend { get; set; }

    }
}