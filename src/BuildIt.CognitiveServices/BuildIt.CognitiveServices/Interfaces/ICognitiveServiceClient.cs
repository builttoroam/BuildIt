﻿using System.Threading.Tasks;
using BuildIt.CognitiveServices.Common;
using BuildIt.CognitiveServices.Models;
using BuildIt.CognitiveServices.Models.Feeds.Academic;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices.Models.Feeds.Language;
using BuildIt.CognitiveServices.Models.Feeds.Search;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Vision.Contract;
using FaceRectangle = Microsoft.ProjectOxford.Face.Contract.FaceRectangle;

namespace BuildIt.CognitiveServices.Interfaces
{
    public interface ICognitiveServiceClient
    {
        Task<ResultDto<InterpretApiFeeds>> AcademicInterpretApiRequestAsync(AcademicParameters academicParameters);

        Task<ResultDto<SpellCheckApiFeeds>> SpellCheckApiRequestAsync(SpellCheckParameters spellCheckParameters);

        Task<ResultDto<DetectLanguageApiFeeds>> DetectLanguageApiRequestAsync(string subscriptionKey, string jsonString, string contentType = Constants.DefaultContentType, int numberofLanguagesToDetect = 1);

        Task<ResultDto<KeyPhrasesApiFeeds>> KeyPhrasesApiRequestAsync(string subscriptionKey, string jsonString, string contentType = Constants.DefaultContentType);

        Task<ResultDto<SentimentApiFeeds>> SentimentApiRequestAsync(string subscriptionKey, string jsonString, string contentType = Constants.DefaultContentType);

        Task<ResultDto<BreakIntoWordsApiFeeds>> BreakIntoWordsApiRequestAsync(BreakIntoWordsParameters breakIntoWordsParameters);

        Task<ResultDto<BingAutosuggestApiFeeds>> BingAutosuggestApiRequestAsync(string subscriptionKey, string context);

        Task<ResultDto<BingSearchApiFeeds>> BingSearchApiRequestAsync(BingSearchParameters bingSearchParameters);

        Task<ResultDto<AnalysisResult>> ComputerVisionApiRequestAsync(string subscriptionKey, string photoUri);

        Task<ResultDto<Emotion[]>> VisionEmotionApiRequestAsync(string subscriptionKey, string photoUri);

        Task<ResultDto<FaceRectangle[]>> VisionFaceApiCheckAsync(string subscriptionKey, string photoUri);
    }
}