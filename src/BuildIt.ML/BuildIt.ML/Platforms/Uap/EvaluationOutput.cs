using System.Collections.Generic;

namespace BuildIt.ML
{
    /// <summary>
    /// Stores the result of evaluating the input via Windows ML
    /// </summary>
    internal class EvaluationOutput
    {
        public EvaluationOutput(string[] labels)
        {
            ClassLabel = new List<string>();
            Loss = new Dictionary<string, float>();
            foreach (var label in labels)
            {
                Loss[label] = float.NaN;
            }
        }

        public IList<string> ClassLabel { get; set; }

        public IDictionary<string, float> Loss { get; set; }
    }
}