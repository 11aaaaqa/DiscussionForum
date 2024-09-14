namespace Web.MVC.Services
{
    public class SuggestionsService : IReportService, ISuggestionService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SuggestionsService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<bool> DeleteReport(Guid reportId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response =
                await httpClient.DeleteAsync($"http://report-microservice-api:8080/api/Report/DeleteReportById/{reportId}");
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> DeleteSuggestedTopic(Guid suggestedTopicId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://topic-microservice-api:8080/api/SuggestTopic/RejectSuggestedTopic/{suggestedTopicId}");
            if(response.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteSuggestedDiscussion(Guid suggestedDiscussionId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/RejectSuggestedDiscussion/{suggestedDiscussionId}");
            if (response.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteSuggestedComment(Guid suggestedCommentId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://comment-microservice-api:8080/api/SuggestComment/RejectSuggestedComment/{suggestedCommentId}");
            if (response.IsSuccessStatusCode) return true;
            return false;
        }
    }
}
