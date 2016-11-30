namespace BuildIt.CognitiveServicesClient.Models.Feeds.InputParameters
{
    public class AcademicParameters
    {
        public string subscriptionKey { get; set; }
        public string content { get; set; }
        public string model { get; set; } = "latest";
        public string attributes { get; set; }
        public int count { get; set; } = 10;
        public int offset { get; set; } = 0;
        public int complete { get; set; } = 1;
        public int timeout { get; set; } = 1000;
    }
}
