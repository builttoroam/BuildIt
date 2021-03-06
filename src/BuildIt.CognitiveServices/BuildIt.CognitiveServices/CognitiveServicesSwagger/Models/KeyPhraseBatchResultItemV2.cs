// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class KeyPhraseBatchResultItemV2
    {
        /// <summary>
        /// Initializes a new instance of the KeyPhraseBatchResultItemV2 class.
        /// </summary>
        public KeyPhraseBatchResultItemV2() { }

        /// <summary>
        /// Initializes a new instance of the KeyPhraseBatchResultItemV2 class.
        /// </summary>
        /// <param name="keyPhrases">A list of representative words or
        /// phrases. The number of key phrases returned is proportional to
        /// the number of words in the input document.</param>
        /// <param name="id">Unique document identifier.</param>
        public KeyPhraseBatchResultItemV2(System.Collections.Generic.IList<string> keyPhrases = default(System.Collections.Generic.IList<string>), string id = default(string))
        {
            KeyPhrases = keyPhrases;
            Id = id;
        }

        /// <summary>
        /// Gets or sets a list of representative words or phrases. The number
        /// of key phrases returned is proportional to the number of words in
        /// the input document.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "keyPhrases")]
        public System.Collections.Generic.IList<string> KeyPhrases { get; set; }

        /// <summary>
        /// Gets or sets unique document identifier.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

    }
}
