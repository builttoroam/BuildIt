using Android.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace BuildIt.ML
{
    public static class StreamExtensions
    {
        public static async Task<float[]> ToCustomVisionInputAsync(this Stream stream)
        {
            var floatValues = new float[227 * 227 * 3];

            using (var bitmap = await BitmapFactory.DecodeStreamAsync(stream))
            {
                using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, 227, 227, false))
                {
                    using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                    {
                        var intValues = new int[227 * 227];
                        resizedBitmap.GetPixels(intValues, 0, 227, 0, 0, 227, 227);
                        for (int i = 0; i < intValues.Length; ++i)
                        {
                            var val = intValues[i];
                            floatValues[i * 3 + 0] = ((val & 0xFF) - 104);
                            floatValues[i * 3 + 1] = (((val >> 8) & 0xFF) - 117);
                            floatValues[i * 3 + 2] = (((val >> 16) & 0xFF) - 123);
                        }

                        resizedBitmap.Recycle();
                    }

                    scaledBitmap.Recycle();
                }

                bitmap.Recycle();
            }

            return floatValues;
        }
    }
}