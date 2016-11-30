using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.Service.Interfaces;
using MvvmCross.Core.ViewModels;

namespace CognitiveServicesDemo.ViewModels
{
    public class BingSpeechViewModel : MvxViewModel
    {
        public IBingSpeech BingSpeech;
        public BingSpeechViewModel(IBingSpeech bingSpeech)
        {
            BingSpeech = bingSpeech;
        }
    }
}
