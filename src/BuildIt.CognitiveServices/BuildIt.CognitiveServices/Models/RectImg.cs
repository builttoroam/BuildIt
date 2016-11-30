using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.CognitiveServices.Models
{
    public class RectImg :Image
    {
        private List<Rectangle> rectangle;

        public List<Rectangle> Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
    }
}
