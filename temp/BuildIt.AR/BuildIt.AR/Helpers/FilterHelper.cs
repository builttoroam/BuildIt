namespace BuildIt.AR.Helpers
{
    public static class FilterHelper
    {
        public static float[] LowPassFilter(float alpha, float[] input, float[] output)
        {
            if (output == null || (output != null && input.Length != output.Length))
            {
                return input;
            }
            var result = new float[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                result[i] = output[i] + alpha * (input[i] - output[i]);
            }
            return result;
        }
    }
}
