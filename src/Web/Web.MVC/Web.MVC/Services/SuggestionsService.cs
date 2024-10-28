namespace Web.MVC.Services
{
    public class SuggestionsService : IReportService, ISuggestionService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public SuggestionsService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }
        public async Task<bool> DeleteReport(Guid reportId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response =
                await httpClient.DeleteAsync($"{url}/api/Report/DeleteReportById/{reportId}");
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> DeleteSuggestedTopic(Guid suggestedTopicId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestTopic/RejectSuggestedTopic/{suggestedTopicId}");
            if(response.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteSuggestedDiscussion(Guid suggestedDiscussionId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestDiscussion/RejectSuggestedDiscussion/{suggestedDiscussionId}");
            if (response.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteSuggestedComment(Guid suggestedCommentId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestComment/RejectSuggestedComment/{suggestedCommentId}");
            if (response.IsSuccessStatusCode) return true;
            return false;
        }
    }
}
