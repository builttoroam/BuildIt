using Newtonsoft.Json.Serialization;

namespace BuildIt.Web.Utitlites
{
    /// <summary>
    /// 
    /// </summary>
    public class LowerCaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
