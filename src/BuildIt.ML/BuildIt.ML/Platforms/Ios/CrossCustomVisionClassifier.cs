namespace BuildIt.ML
{
    public static class CrossCustomVisionClassifier
    {
        private static ICustomVisionClassifier instance;

        public static ICustomVisionClassifier Instance => instance ?? (instance = new CustomVisionClassifier());
    }
}