using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildIt.CognitiveServices.Common;
using BuildIt.CognitiveServices.Models;
using BuildIt.CognitiveServices.Models.Feeds.Academic;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using Newtonsoft.Json;

namespace BuildIt.CognitiveServices
{
    public class CognitiveServiceKnowledge
    {
        public async Task<ResultDto<InterpretApiFeeds>> AcademicInterpretApiRequestAsync(AcademicParameters academicParameters)
        {
            var resultDto = new ResultDto<InterpretApiFeeds>();
            try
            {
                var client = new HttpClient();
                
                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, academicParameters.subscriptionKey);
                var queryString = $"query={academicParameters.content}&offset={academicParameters.offset}&attributes={academicParameters.attributes}&count={academicParameters.count}&model={academicParameters.model}&complete={academicParameters.complete}&timeout={academicParameters.timeout}";
                var url = Constants.AcademicInterpretApi + queryString;

                var jsonResult = await client.GetStringAsync(url);
                var feed = JsonConvert.DeserializeObject<InterpretApiFeeds>(jsonResult);
                
                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.error?.message;
                resultDto.StatusCode = feed.error?.code;
                resultDto.Success = feed.error == null;
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Console.WriteLine($"{ex}");
            }
            return resultDto;
        }
    }
}
