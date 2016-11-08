namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    public interface IDirectLineApiConversationProvider
    {
        string ConversationId { get; }

        DirectLinkApiClient ConversationClient { get; }
    }
}
