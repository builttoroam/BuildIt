using System;

namespace BuildIt.CognitiveServicesClient.Models.Interfaces
{
    public interface IResultDto
    {
        bool Success { get; set; }
        string ErrorMessage { get; set; }
        string StatusCode { get; set; }
        Exception Exception { get; set; } 
    }
    public interface IResultDto<T> : IResultDto
    {
        T Result { get; set; }
    }
}