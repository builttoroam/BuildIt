using BuildIt.ML.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuildIt.ML.Interfaces
{
    public interface ICustomVisionClassifier
    {
        Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream);

        Task InitAsync(string modelName, string[] labels);
    }
}