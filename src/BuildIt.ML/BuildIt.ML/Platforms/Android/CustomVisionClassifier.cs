using Android.Graphics;
using Org.Tensorflow;
using Org.Tensorflow.Contrib.Android;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.ML
{
    public class CustomVisionClassifier : ICustomVisionClassifier
    {
        private const string OutputName = "loss";
        private const string InputName = "Placeholder";
        private TensorFlowInferenceInterface inferenceInterface;
        private string[] labels;
        private string DataNormLayerPrefix = "data_bn";
        private bool hasNormalizationLayer;

        public async Task InitAsync(string modelName, string[] labels)
        {
            this.labels = labels;
            var assets = global::Android.App.Application.Context.Assets;

            inferenceInterface = new TensorFlowInferenceInterface(assets, modelName + ".pb");

            var iter = inferenceInterface.Graph().Operations();
            while (iter.HasNext && !hasNormalizationLayer)
            {
                var op = iter.Next() as Operation;
                if (op.Name().Contains(DataNormLayerPrefix))
                {
                    hasNormalizationLayer = true;
                }
            }
            System.Diagnostics.Debug.WriteLine("initialized");
        }

        public async Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream)
        {
            using (var bitmap = await BitmapFactory.DecodeStreamAsync(imageStream))
            {
                var outputs = new float[labels.Length];
                var floatValues = GetBitmapPixels(bitmap, 227, 227, hasNormalizationLayer);

                bitmap.Recycle();
                inferenceInterface.Feed(InputName, floatValues.ToArray(), 1, 227, 227, 3);
                inferenceInterface.Run(new string[] { OutputName });
                inferenceInterface.Fetch(OutputName, outputs);
                var imageClassifications = new List<ImageClassification>();
                for (var i = 0; i < outputs.Length; i++)
                {
                    imageClassifications.Add(new ImageClassification(labels[i], outputs[i]));
                }

                return imageClassifications.AsReadOnly();
            }
        }

        public async Task<IReadOnlyList<ImageClassification>> ClassifyNativeFrameAsync(object obj)
        {
            System.Diagnostics.Debug.WriteLine("cast bytes");
            var bytes = obj as byte[];
            System.Diagnostics.Debug.WriteLine($"decode {bytes.Length}");

            using (var bitmap = await BitmapFactory.DecodeByteArrayAsync(bytes, 0, bytes.Length))
            {
                var outputs = new float[labels.Length];
                System.Diagnostics.Debug.WriteLine("get pixels");
                var floatValues = GetBitmapPixels(bitmap, 227, 227, hasNormalizationLayer);
                System.Diagnostics.Debug.WriteLine("train");
                inferenceInterface.Feed(InputName, floatValues.ToArray(), 1, 227, 227, 3);
                inferenceInterface.Run(new string[] { OutputName });
                inferenceInterface.Fetch(OutputName, outputs);
                var imageClassifications = new List<ImageClassification>();
                for (var i = 0; i < outputs.Length; i++)
                {
                    imageClassifications.Add(new ImageClassification(labels[i], outputs[i]));
                }

                return imageClassifications.AsReadOnly();
            }
        }

        //private float[] GetBitmapPixels(Bitmap bitmap, int width, int height)
        //{
        //    var floatValues = new float[width * height * 3];
        //    var imageMeanB = hasNormalizationLayer ? 0f : 124f;
        //    var imageMeanG = hasNormalizationLayer ? 0f : 117f;
        //    var imageMeanR = hasNormalizationLayer ? 0f : 105f;
        //    System.Diagnostics.Debug.WriteLine($"has normalization layer {hasNormalizationLayer}");
        //    using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, false))
        //    {
        //        using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
        //        {
        //            var intValues = new int[width * height];
        //            resizedBitmap.GetPixels(intValues, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

        //            for (int i = 0; i < intValues.Length; ++i)
        //            {
        //                var val = intValues[i];

        //                floatValues[i * 3 + 0] = ((val & 0xFF) - imageMeanB);
        //                floatValues[i * 3 + 1] = (((val >> 8) & 0xFF) - imageMeanG);
        //                floatValues[i * 3 + 2] = (((val >> 16) & 0xFF) - imageMeanR);
        //            }

        //            resizedBitmap.Recycle();
        //        }

        //        scaledBitmap.Recycle();
        //    }

        //    return floatValues;
        //}

        private static float ImageMeanR()
        {
  
                    return 123.0f;
        }

        private static float ImageMeanG()
        {

                    return 117.0f;
 
        }

        private static float ImageMeanB()
        {

                    return 104.0f;

        }

        public static float[] GetBitmapPixels(Bitmap bitmap, int width, int height, bool hasNormalizationLayer)
        {
            var floatValues = new float[width * height * 3];
            var imageMeanB = hasNormalizationLayer ? 0f : ImageMeanB();
            var imageMeanG = hasNormalizationLayer ? 0f : ImageMeanG();
            var imageMeanR = hasNormalizationLayer ? 0f : ImageMeanR();

            using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, false))
            {
                using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                {
                    var intValues = new int[width * height];
                    resizedBitmap.GetPixels(intValues, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

                    for (int i = 0; i < intValues.Length; ++i)
                    {
                        var val = intValues[i];

                        floatValues[i * 3 + 0] = ((val & 0xFF) - imageMeanB) / 1f;
                        floatValues[i * 3 + 1] = (((val >> 8) & 0xFF) - imageMeanG) / 1f;
                        floatValues[i * 3 + 2] = (((val >> 16) & 0xFF) - imageMeanR) / 1f;
                    }

                    resizedBitmap.Recycle();
                }

                scaledBitmap.Recycle();
            }

            return floatValues;
        }
    }
}