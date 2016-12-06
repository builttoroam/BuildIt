// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices.Models
{
    using System.Linq;

    public partial class Error
    {
        /// <summary>
        /// Initializes a new instance of the Error class.
        /// </summary>
        public Error() { }

        /// <summary>
        /// Initializes a new instance of the Error class.
        /// </summary>
        /// <param name="code">Unique error code identifying the error</param>
        /// <param name="message">Error message</param>
        /// <param name="innerError">Error details</param>
        public Error(string code = default(string), string message = default(string), InternalError innerError = default(InternalError))
        {
            Code = code;
            Message = message;
            InnerError = innerError;
        }

        /// <summary>
        /// Gets or sets unique error code identifying the error
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets error message
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets error details
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "innerError")]
        public InternalError InnerError { get; set; }

    }
}
