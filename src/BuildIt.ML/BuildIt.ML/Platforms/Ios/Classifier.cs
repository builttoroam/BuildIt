using CoreML;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.ML.Platforms.Ios
{
    public class Classifier
    {
        private const string CoreMLModelExtension = "mlmodelc";
        private MLModel model;
        IEnumerable<Feature> outputFeatures;

        public Task InitAsync(string modelName, IEnumerable<Feature> outputFeatures)
        {
            this.outputFeatures = outputFeatures;
            var assetPath = NSBundle.MainBundle.GetUrlForResource(modelName, CoreMLModelExtension);
            // TODO: handle when there's an error
            model = MLModel.Create(assetPath, out NSError error);
            return Task.CompletedTask;
        }

        public async Task<IMLFeatureProvider> ClassifyAsync(IEnumerable<Feature> inputFeatures)
        {
            var inputs = new Dictionary<string, NSObject>();
            foreach (var inputFeature in inputFeatures)
            {
                switch (inputFeature.Type)
                {
                    case MLFeatureType.Int64:
                        if (inputFeature is LongFeatureValue longInputFeature)
                        {
                            inputs[longInputFeature.Name] = MLFeatureValue.Create(longInputFeature.Value);
                        }

                        break;

                    case MLFeatureType.Double:
                        if (inputFeature is DoubleFeatureValue doubleInputFeature)
                        {
                            inputs[doubleInputFeature.Name] = MLFeatureValue.Create(doubleInputFeature.Value);
                        }

                        break;

                    case MLFeatureType.String:
                        if (inputFeature is StringFeatureValue stringInputFeature)
                        {
                            inputs[stringInputFeature.Name] = MLFeatureValue.Create(stringInputFeature.Value);
                        }

                        break;

                    case MLFeatureType.Image:
                        if (inputFeature is ImageFeatureValue imageInputFeature)
                        {
                            inputs[imageInputFeature.Name] = MLFeatureValue.Create(imageInputFeature.Value);
                        }

                        break;

                    case MLFeatureType.MultiArray:
                        if (inputFeature is MultiArrayInputFeature multiArrayInputFeature)
                        {
                            inputs[multiArrayInputFeature.Name] = MLFeatureValue.Create(multiArrayInputFeature.Value);
                        }

                        break;

                    case MLFeatureType.Dictionary:
                        if (inputFeature is DictionaryFeatureValue dictionaryInputFeature)
                        {
                            inputs[dictionaryInputFeature.Name] = MLFeatureValue.Create(dictionaryInputFeature.Value, out NSError inputError);

                            // TODO: handle error
                        }

                        break;
                }
            }

            var inputFeatureProvider = new MLDictionaryFeatureProvider(NSDictionary<NSString, NSObject>.FromObjectsAndKeys(inputs.Values.ToArray(), inputs.Keys.ToArray()), out NSError error);

            // TODO: handle errors
            var outputFeatures = model.GetPrediction(inputFeatureProvider, out error);
            // TODO: handle errors

            // TODO: handle parsing the output
            return outputFeatures;
        }
    }
}