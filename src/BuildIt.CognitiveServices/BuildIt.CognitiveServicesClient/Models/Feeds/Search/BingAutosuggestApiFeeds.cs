using System.Collections.Generic;

namespace BuildIt.CognitiveServicesClient.Models.Feeds.Search
{
    public class BingAutosuggestApiFeeds
    {
        public string _type { get; set; }
        public QueryContext queryContext { get; set; }
        public List<SuggestionGroup> suggestionGroups { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }

    public class QueryContext
    {
        public string originalQuery { get; set; }
    }

    public class SearchSuggestion
    {
        public string url { get; set; }
        public string displayText { get; set; }
        public string query { get; set; }
        public string searchKind { get; set; }
    }

    public class SuggestionGroup
    {
        public string name { get; set; }
        public List<SearchSuggestion> searchSuggestions { get; set; }
    }
}
