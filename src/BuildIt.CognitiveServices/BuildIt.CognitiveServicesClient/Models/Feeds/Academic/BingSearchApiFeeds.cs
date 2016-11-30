using System.Collections.Generic;

namespace BuildIt.CognitiveServicesClient.Models.Feeds.Academic
{
    public class BingSearchApiFeeds
    {
        public string _type { get; set; }
        public WebPages webPages { get; set; }
        public Images images { get; set; }
        public News news { get; set; }
        public RelatedSearches relatedSearches { get; set; }
        public Videos videos { get; set; }
        public RankingResponse rankingResponse { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }
    public class About
    {
        public string name { get; set; }
    }

    public class DeepLink
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public List<About> about { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public List<DeepLink> deepLinks { get; set; }
        public string dateLastCrawled { get; set; }
    }

    public class WebPages
    {
        public string webSearchUrl { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<Value> value { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Value2
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
        public Thumbnail thumbnail { get; set; }
    }

    public class Images
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public bool isFamilyFriendly { get; set; }
        public List<Value2> value { get; set; }
        public bool displayShoppingSourcesBadges { get; set; }
        public bool displayRecipeSourcesBadges { get; set; }
    }

    public class Thumbnail2
    {
        public string contentUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Image
    {
        public string contentUrl { get; set; }
        public Thumbnail2 thumbnail { get; set; }
    }

    public class About2
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Provider
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class About3
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Provider2
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class ClusteredArticle
    {
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public List<About3> about { get; set; }
        public List<Provider2> provider { get; set; }
        public string datePublished { get; set; }
        public string category { get; set; }
    }

    public class Thumbnail3
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Video
    {
        public string name { get; set; }
        public string motionThumbnailUrl { get; set; }
        public Thumbnail3 thumbnail { get; set; }
    }

    public class Value3
    {
        public string name { get; set; }
        public string url { get; set; }
        public Image image { get; set; }
        public string description { get; set; }
        public List<About2> about { get; set; }
        public List<Provider> provider { get; set; }
        public string datePublished { get; set; }
        public string category { get; set; }
        public List<ClusteredArticle> clusteredArticles { get; set; }
        public Video video { get; set; }
    }

    public class News
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public List<Value3> value { get; set; }
    }

    public class Value4
    {
        public string text { get; set; }
        public string displayText { get; set; }
        public string webSearchUrl { get; set; }
    }

    public class RelatedSearches
    {
        public string id { get; set; }
        public List<Value4> value { get; set; }
    }

    public class Publisher
    {
        public string name { get; set; }
    }

    public class Thumbnail4
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Value5
    {
        public string name { get; set; }
        public string description { get; set; }
        public string webSearchUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string datePublished { get; set; }
        public List<Publisher> publisher { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string duration { get; set; }
        public string motionThumbnailUrl { get; set; }
        public string embedHtml { get; set; }
        public bool allowHttpsEmbed { get; set; }
        public int viewCount { get; set; }
        public Thumbnail4 thumbnail { get; set; }
        public bool allowMobileEmbed { get; set; }
        public bool isSuperfresh { get; set; }
    }

    public class Videos
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public bool isFamilyFriendly { get; set; }
        public List<Value5> value { get; set; }
        public string scenario { get; set; }
    }

    public class Value6
    {
        public string id { get; set; }
    }

    public class Item
    {
        public string answerType { get; set; }
        public int resultIndex { get; set; }
        public Value6 value { get; set; }
    }

    public class Mainline
    {
        public List<Item> items { get; set; }
    }

    public class Value7
    {
        public string id { get; set; }
    }

    public class Item2
    {
        public string answerType { get; set; }
        public Value7 value { get; set; }
    }

    public class Sidebar
    {
        public List<Item2> items { get; set; }
    }

    public class RankingResponse
    {
        public Mainline mainline { get; set; }
        public Sidebar sidebar { get; set; }
    }
}
