namespace BuildIt.CognitiveServicesClient.Models.Feeds.InputParameters
{
    public class SpellCheckParameters
    {
        public string subscriptionKey { get; set; }
        public string content { get; set; }
        public string mode { get; set; } = "proof";
        public string mkt { get; set; } = "en-us";
    }
}
