using System;

namespace CognitiveServicesDemo.Service.Interfaces
{
    public interface IBingSpeech
    {
        void StartSpeech(Action<string> action);
    }
}