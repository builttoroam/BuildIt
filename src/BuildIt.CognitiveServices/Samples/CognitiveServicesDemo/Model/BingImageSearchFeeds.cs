using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    public class BingImageSearchFeeds
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<BingImageSearchValue> value { get; set; }
        public List<QueryExpansion> queryExpansions { get; set; }
        public int nextOffsetAddCount { get; set; }
        public List<PivotSuggestion> pivotSuggestions { get; set; }
        public bool displayShoppingSourcesBadges { get; set; }
        public bool displayRecipeSourcesBadges { get; set; }
        public List<SimilarTerm> similarTerms { get; set; }
    }

    public class Instrumentation
    {
        public string pageLoadPingUrl { get; set; }
    }

    public class BingImageSearchThumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class InsightsSourcesSummary
    {
        public int shoppingSourcesCount { get; set; }
        public int recipeSourcesCount { get; set; }
    }

    public class BingImageSearchValue
    {
        public string name { get; set; }
        public string webSearchUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public BingImageSearchThumbnail thumbnail { get; set; }
        public string imageInsightsToken { get; set; }
        public InsightsSourcesSummary insightsSourcesSummary { get; set; }
        public string imageId { get; set; }
        public string accentColor { get; set; }
    }

    public class BingImageSearchThumbnail2
    {
        public string thumbnailUrl { get; set; }
    }

    public class QueryExpansion
    {
        public string text { get; set; }
        public string displayText { get; set; }
        public string webSearchUrl { get; set; }
        public string searchLink { get; set; }
        public BingImageSearchThumbnail2 thumbnail { get; set; }
    }

    public class BingImageSearchThumbnail3
    {
        public string thumbnailUrl { get; set; }
    }

    public class BingImageSearchSuggestion
    {
        public string text { get; set; }
        public string displayText { get; set; }
        public string webSearchUrl { get; set; }
        public string searchLink { get; set; }
        public BingImageSearchThumbnail3 thumbnail { get; set; }
    }

    public class PivotSuggestion
    {
        public string pivot { get; set; }
        public List<BingImageSearchSuggestion> suggestions { get; set; }
    }

    public class BingImageSearchThumbnail4
    {
        public string url { get; set; }
    }

    public class SimilarTerm
    {
        public string text { get; set; }
        public string displayText { get; set; }
        public string webSearchUrl { get; set; }
        public BingImageSearchThumbnail4 thumbnail { get; set; }
    }
}
