using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CommentMicroservice.Api.DTOs;
using CommentMicroservice.Api.Models;

namespace CommentMicroservice.IntegrationTests
{
    public class ControllersTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient client;

        public ControllersTests(TestingWebAppFactory<Program> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task SuggestCommentAsync_ReturnsOkWithSuggestedComment()
        {
            var model = new SuggestCommentDto
                { Content = "Content", CreatedBy = "CreatedBy", DiscussionId = Guid.NewGuid() };
            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var result = await client.PostAsync("api/SuggestComment/Suggest", jsonContent);

            result.EnsureSuccessStatusCode();
            var suggestedComment = await result.Content.ReadFromJsonAsync<SuggestedComment>();
            Assert.NotNull(suggestedComment);
            Assert.Equal(model.Content, suggestedComment.Content);
            Assert.Equal(model.CreatedBy, suggestedComment.CreatedBy);
            Assert.Equal(model.DiscussionId, suggestedComment.DiscussionId);
        }

        [Fact]
        public async Task AcceptSuggestedCommentAsync_ReturnsBadRequest()
        {
            var id = Guid.Empty;
            var result = await client.DeleteAsync($"api/SuggestComment/AcceptSuggestedComment/{id}");

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task AcceptSuggestedCommentAsync_ReturnsOk()
        {
            var id = new Guid("6551f496-0ebf-4417-89c8-a8de47579923");
            var result = await client.DeleteAsync($"api/SuggestComment/AcceptSuggestedComment/{id}");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task RejectSuggestedCommentAsync_ReturnsBadRequest()
        {
            var id = Guid.Empty;
            var result = await client.DeleteAsync($"api/SuggestComment/RejectSuggestedComment/{id}");

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task RejectSuggestedCommentAsync_ReturnsOk()
        {
            var id = new Guid("67e7243e-cf1b-411b-bd73-7bb63be07d01");
            var result = await client.DeleteAsync($"api/SuggestComment/RejectSuggestedComment/{id}");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
