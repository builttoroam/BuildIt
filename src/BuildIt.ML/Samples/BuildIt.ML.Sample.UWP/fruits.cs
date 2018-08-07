using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// 654f7a45-abff-4d07-9c4d-7deae648766e_ff48cb29-ec75-4211-84bd-8d7dfd00ce36

namespace BuildIt.ML.Sample.UWP
{
    public sealed class _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelInput
    {
        public VideoFrame data { get; set; }
    }

    public sealed class _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> loss { get; set; }
        public _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelOutput()
        {
            this.classLabel = new List<string>();
            this.loss = new Dictionary<string, float>()
            {
                { "apple", float.NaN },
                { "banana", float.NaN },
                { "pineapple", float.NaN },
            };
        }
    }

    public sealed class _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36Model
    {
        private LearningModelPreview learningModel;
        public static async Task<_x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36Model> Create_x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36Model(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36Model model = new _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36Model();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<_x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelOutput> EvaluateAsync(_x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelInput input) {
            _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelOutput output = new _x0036_54f7a45_x002D_abff_x002D_4d07_x002D_9c4d_x002D_7deae648766e_ff48cb29_x002D_ec75_x002D_4211_x002D_84bd_x002D_8d7dfd00ce36ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("data", input.data);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("loss", output.loss);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
