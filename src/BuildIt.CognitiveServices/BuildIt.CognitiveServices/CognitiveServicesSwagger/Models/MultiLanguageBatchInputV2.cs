// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class MultiLanguageBatchInputV2
    {
        /// <summary>
        /// Initializes a new instance of the MultiLanguageBatchInputV2 class.
        /// </summary>
        public MultiLanguageBatchInputV2() { }

        /// <summary>
        /// Initializes a new instance of the MultiLanguageBatchInputV2 class.
        /// </summary>
        public MultiLanguageBatchInputV2(System.Collections.Generic.IList<MultiLanguageInputV2> documents = default(System.Collections.Generic.IList<MultiLanguageInputV2>))
        {
            Documents = documents;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "documents")]
        public System.Collections.Generic.IList<MultiLanguageInputV2> Documents { get; set; }

    }
}
