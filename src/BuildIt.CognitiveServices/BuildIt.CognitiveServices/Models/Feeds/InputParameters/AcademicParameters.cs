namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class AcademicParameters
    {
        public string SubscriptionKey { get; set; }
        public string Query { get; set; }
        public string Model { get; set; } = "latest";
        public int Count { get; set; } = 10;
        public int Offset { get; set; } = 0;
        public int Complete { get; set; } = 1;
        public int Timeout { get; set; } = 1000;
    }
}