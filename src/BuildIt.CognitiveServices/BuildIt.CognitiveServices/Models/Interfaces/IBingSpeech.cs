using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.CognitiveServicesClient.Models.Interfaces
{
    public interface IBingSpeech
    {
        void StartSpeech(Action<string> action);
    }
}
