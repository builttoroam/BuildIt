using System;
using BuildIt.CognitiveServicesClient.Models.Interfaces;

namespace BuildIt.CognitiveServicesClient.Models
{
    public class ResultDto<T> : IResultDto<T>
    {
        public bool Success { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusCode { get; set; }
        public Exception Exception { get; set; }
    }

    public class ResultDto : IResultDto
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusCode { get; set; }
        public Exception Exception { get; set; }
    }
}
