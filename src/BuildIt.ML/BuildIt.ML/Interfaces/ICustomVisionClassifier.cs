using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuildIt.ML
{
    public interface ICustomVisionClassifier
    {
        /// <summary>
        /// Classifies an image stream
        /// </summary>
        /// <param name="imageStream">The image stream</param>
        /// <returns>The classification results</returns>
        Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream);

        /// <summary>
        /// Classifies a frame received from a camera feed
        /// </summary>
        /// <param name="obj">The frame received from a camera feed</param>
        /// <returns>The classification results</returns>
        Task<IReadOnlyList<ImageClassification>> ClassifyNativeFrameAsync(object obj);

        /// <summary>
        /// Initializes the classifier using the provided model
        /// </summary>
        /// <param name="modelName">Name of the model file. On UWP, this should be in the Assets folder</param>
        /// <param name="labels">Output labels</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task InitAsync(string modelName, string[] labels);
    }
}