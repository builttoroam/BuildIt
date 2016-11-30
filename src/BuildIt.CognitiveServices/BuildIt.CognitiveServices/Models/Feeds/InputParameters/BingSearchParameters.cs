namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class BingSearchParameters
    {
        public string subscriptionKey { get; set; }
        public string content { get; set; }
        public int count { get; set; } = 10;
        public int offset { get; set; } = 0;
        public string mkt { get; set; } = "en-us";
        public string safesearch { get; set; } = "Moderate";
    }
}