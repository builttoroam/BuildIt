namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class AcademicParameters
    {
        public string SubscriptionKey { get; set; }
        /// <summary>
        /// Query entered by user. If complete is set to 1, query will be interpreted as a prefix for generating query auto-completion suggestions.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Name of the model that you wish to query. Currently, the value defaults to "latest".
        /// </summary>
        public string Model { get; set; } = "latest";
        /// <summary>
        /// Maximum number of interpretations to return. Default value is 10
        /// </summary>
        public int Count { get; set; } = 10;
        /// <summary>
        /// Index of the first interpretation to return. For example, count=2&offset=0 returns interpretations 0 and 1. count=2&offset=2 returns interpretations 2 and 3. Default value is 0
        /// </summary>
        public int Offset { get; set; } = 0;
        /// <summary>
        /// 1 means that auto-completion suggestions are generated based on the grammar and graph data. Default value is 1
        /// </summary>
        public int Complete { get; set; } = 1;
        /// <summary>
        /// Timeout in milliseconds. Only interpretations found before the timeout has elapsed are returned. Default value is 10000
        /// </summary>
        public int Timeout { get; set; } = 1000;
    }
}