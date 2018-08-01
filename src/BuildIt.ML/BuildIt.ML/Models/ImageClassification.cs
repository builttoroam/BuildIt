namespace BuildIt.ML
{
    public class ImageClassification
    {
        public ImageClassification(string label, double confidence)
        {
            Label = label;
            Confidence = confidence;
        }

        public string Label { get; }

        public double Confidence { get; }
    }
}