using System.Net;
using System.Net.Http.Json;
using DiscussionMicroservice.Api.Models;

namespace DiscussionMicroservice.IntegrationTests
{
    public class DiscussionControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient client;

        public DiscussionControllerIntegrationTests(TestingWebAppFactory<Program> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task GetDiscussionByIdAsync_ReturnsBadRequest()
        {
            var response = await client.GetAsync($"api/Discussion/GetDiscussionById?id={Guid.Empty}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetDiscussionByIdAsync_ReturnsOkWithDiscussion()
        {
            Guid id = new Guid("8077167c-d724-4258-8451-b617dc4bdfec");
            var response = await client.GetAsync($"api/Discussion/GetDiscussionById?id={id}");

            response.EnsureSuccessStatusCode();
            var discussion = await response.Content.ReadFromJsonAsync<Discussion>();
            Assert.NotNull(discussion);
            Assert.Equal(id, discussion.Id);
        }

        [Fact]
        public async Task GetDiscussionsByTopicName_ReturnsOkWithListOfDiscussions()
        {
            var topicName = "TestTopicName";
            var response = await client.GetAsync($"api/Discussion/GetDiscussionsByTopicName?topicName={topicName}");

            response.EnsureSuccessStatusCode();
            var discussions = await response.Content.ReadFromJsonAsync<List<Discussion>>();
            Assert.Equal(2, discussions.Count);
            Assert.Equal(topicName, discussions[0].TopicName);
            Assert.Equal(topicName, discussions[1].TopicName);
        }
    }
}
