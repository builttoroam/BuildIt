namespace BuildIt.Bot.Client.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EndpointRouteDetails
    {
        /// <summary>
        /// 
        /// </summary>
        public string BaseServiceUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceAffix { get; set; } = "api";
        /// <summary>
        /// 
        /// </summary>
        public string RegisterPushRoute { get; set; } = "registerpush";
        /// <summary>
        /// 
        /// </summary>
        public string DeregisterPushRoute { get; set; } = "deregisterpush";
    }
}
