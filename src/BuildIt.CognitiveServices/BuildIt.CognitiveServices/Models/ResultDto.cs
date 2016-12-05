using System;
using BuildIt.CognitiveServices.Models.Interfaces;

namespace BuildIt.CognitiveServices.Models
{
    public class ResultDto<T> : ResultDto, IResultDto<T>
    {
        public T Result { get; set; }
    }

    public class ResultDto : IResultDto
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusCode { get; set; }
        public Exception Exception { get; set; }
    }
}