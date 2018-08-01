using System.Collections.Generic;

namespace BuildIt.ML
{
    internal class CustomVisionOutput
    {
        public IList<string> ClassLabel { get; set; }

        public IDictionary<string, float> Loss { get; set; }

        public CustomVisionOutput(string[] labels)
        {
            ClassLabel = new List<string>();
            Loss = new Dictionary<string, float>();
            foreach (var label in labels)
            {
                Loss[label] = float.NaN;
            }
        }
    }
}