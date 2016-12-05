namespace BuildIt.CognitiveServices.Models.Feeds.InputParameters
{
    public class SpellCheckParameters
    {
        /// <summary>
        /// Subscription key
        /// </summary>
        public string subscriptionKey { get; set; }
        /// <summary>
        /// Media type of the body sent to the API.
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// Mode of spellcheck: "proof" and "spell". Default as "proof".
        /// </summary>
        public string mode { get; set; } = "proof";
        /// <summary>
        /// For proof mode, only support en-us, es-es, pt-br, For spell mode, support all language codes. Default as en-us.
        /// </summary>
        public string mkt { get; set; } = "en-us";
    }
}