using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.Model
{
    public class FacesResult
    {
        public int Age { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public string Gender { get; set; }
    }
}
