namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDirectLineApiConversationProvider
    {
        /// <summary>
        /// 
        /// </summary>
        string ConversationId { get; }

        /// <summary>
        /// 
        /// </summary>
        DirectLinkApiClient ConversationClient { get; }
    }
}
