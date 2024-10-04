using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DiscussionMicroservice.Api.DTOs;
using DiscussionMicroservice.Api.Models;

namespace DiscussionMicroservice.IntegrationTests
{
    public class ControllersIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient client;

        public ControllersIntegrationTests(TestingWebAppFactory<Program> factory)
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
            var response = await client.GetAsync($"api/Discussion/GetDiscussionsByTopicName?topicName={topicName}&pageSize=3&pageNumber=1");

            response.EnsureSuccessStatusCode();
            var discussions = await response.Content.ReadFromJsonAsync<List<Discussion>>();
            Assert.Equal(2, discussions.Count);
            Assert.Equal(topicName, discussions[0].TopicName);
            Assert.Equal(topicName, discussions[1].TopicName);
        }

        [Fact]
        public async Task GetAllSuggestedDiscussionsAsync_ReturnOkWithListOfSuggestedDiscussions()
        {
            var response = await client.GetAsync("api/SuggestDiscussion/GetAllSuggestedDiscussions");

            response.EnsureSuccessStatusCode();
            var suggestedDiscussions = await response.Content.ReadFromJsonAsync<List<SuggestedDiscussion>>();
            Assert.NotNull(suggestedDiscussions);
        }

        [Fact]
        public async Task GetSuggestedDiscussionByIdAsync_ReturnsBadRequest()
        {
            var response = await client.GetAsync($"api/SuggestDiscussion/GetSuggestedDiscussionById?id={Guid.Empty}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSuggestedDiscussionByIdAsync_ReturnsOkWithSuggestedDiscussion()
        {
            var id = new Guid("1fd24884-7dd7-40a9-bfe5-f8247a5e3bc7");
            var response = await client.GetAsync($"api/SuggestDiscussion/GetSuggestedDiscussionById?id={id}");

            response.EnsureSuccessStatusCode();
            var suggestedDiscussion = await response.Content.ReadFromJsonAsync<SuggestedDiscussion>();
            Assert.NotNull(suggestedDiscussion);
            Assert.Equal(id, suggestedDiscussion.Id);
        }

        [Fact]
        public async Task SuggestToCreateDiscussionAsync_ReturnsOk()
        {
            var model = new DiscussionDto { Content = "TestContent1212", CreatedBy = "TestCreatedBy1212", Title = "TestTitle1212" };
            using StringContent content = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/SuggestDiscussion/SuggestToCreate?topicName=TestTopicName1212", content);

            response.EnsureSuccessStatusCode();
            var resp = await client.GetAsync("api/SuggestDiscussion/GetAllSuggestedDiscussions");
            resp.EnsureSuccessStatusCode();
            var suggestedDiscussions = await resp.Content.ReadFromJsonAsync<List<SuggestedDiscussion>>();
            var addedSuggestedDiscussion = suggestedDiscussions.Where(x => x.Content == "TestContent1212")
                .Where(x => x.CreatedBy == "TestCreatedBy1212").Where(x => x.Title == "TestTitle1212")
                .SingleOrDefault();
            Assert.NotNull(addedSuggestedDiscussion);
        }

        [Fact]
        public async Task RejectSuggestedDiscussionAsync_ReturnsOk()
        {
            var id = new Guid("0629b192-d453-4dc2-86f5-d8bbd7fb0955");
            var response = await client.DeleteAsync($"api/SuggestDiscussion/RejectSuggestedDiscussion/{id}");

            response.EnsureSuccessStatusCode();
            var resp = await client.GetAsync($"api/SuggestDiscussion/GetSuggestedDiscussionById?id={id}");
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task AcceptSuggestedDiscussionAsync_ReturnsOk()
        {
            var id = new Guid("8bb9b1c2-4112-4b9c-b67b-8775a0ca6584");

            var response = await client.DeleteAsync($"api/SuggestDiscussion/AcceptSuggestedDiscussion/{id}");

            response.EnsureSuccessStatusCode();
            var resp = await client.GetAsync($"api/SuggestDiscussion/GetSuggestedDiscussionById?id={id}");
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

            var resp2 = await client.GetAsync($"api/Discussion/GetDiscussionById?id={id}");
            resp2.EnsureSuccessStatusCode();
            var discussion = await resp2.Content.ReadFromJsonAsync<Discussion>();
            Assert.NotNull(discussion);
            Assert.Equal(id, discussion.Id);
        }

        [Fact]
        public async Task DeleteDiscussionByIdAsync_ReturnsOk()
        {
            var discussionId = new Guid("5a38b400-8be8-466c-a0ed-62249bc7811b");

            var response = await client.DeleteAsync($"api/Discussion/DeleteDiscussionById/{discussionId}");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteDiscussionByIdAsync_ReturnsBadRequest()
        {
            var discussionId = Guid.NewGuid();

            var response = await client.DeleteAsync($"api/Discussion/DeleteDiscussionById/{discussionId}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetDiscussionsByIdsAsync_ReturnsOkWithListOfDiscussions()
        {
            var ids = new Guid[]
            {
                new ("8077167c-d724-4258-8451-b617dc4bdfec"), new ("5a38b400-8be8-466c-a0ed-62249bc7811b"), Guid.NewGuid()
            };

            var response = await client.GetAsync($"api/Discussion/GetDiscussionsByIds?ids[]={ids[0]}&ids[]={ids[1]}&ids[]={ids[2]}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.IsSuccessStatusCode);
            var returnedDiscussions = await response.Content.ReadFromJsonAsync<List<Discussion>>();
            Assert.NotNull(returnedDiscussions);
            Assert.Equal(2, returnedDiscussions.Count);
        }

        [Fact]
        public async Task GetSuggestedDiscussionsByIdsAsync_ReturnsOkWithListOfSuggestedDiscussions()
        {
            var ids = new Guid[]
            {
                new Guid("1fd24884-7dd7-40a9-bfe5-f8247a5e3bc7"), new Guid("0629b192-d453-4dc2-86f5-d8bbd7fb0955"), Guid.NewGuid()
            };

            var response = await client.GetAsync
                ($"api/SuggestDiscussion/GetSuggestedDiscussionsByIds?ids[]={ids[0]}&ids[]={ids[1]}&ids[]={ids[2]}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.IsSuccessStatusCode);
            var returnedSuggestedDiscussions = await response.Content.ReadFromJsonAsync<List<SuggestedDiscussion>>();
            Assert.NotNull(returnedSuggestedDiscussions);
            Assert.Equal(2, returnedSuggestedDiscussions.Count);
        }

        [Fact]
        public async Task GetSuggestedDiscussionsByUserNameAsync_ReturnsOkWithListOfSuggestedDiscussionsWithSpecifiedCreatedByProperty()
        {
            string userName = "TestCreatedBy555";
            var response = await client.GetAsync ($"api/SuggestDiscussion/GetSuggestedDiscussionsByUserName/{userName}");
            response.EnsureSuccessStatusCode();
            var suggestedDiscussions = await response.Content.ReadFromJsonAsync<List<SuggestedDiscussion>>();
            Assert.Equal(1, suggestedDiscussions.Count);
        }

        [Fact]
        public async Task IncreaseDiscussionRatingByOneAsync_ReturnsBadRequest()
        {
            Guid discussionId = Guid.NewGuid();
            string userNameIncreasedBy = "TestUserName";
            using StringContent jsonContent = new(JsonSerializer.Serialize(new { discussionId, userNameIncreasedBy }),
                Encoding.UTF8, "application/json");
            var response = await client.PatchAsync("api/Discussion/IncreaseDiscussionRatingByOne", jsonContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task IncreaseDiscussionRatingByOneAsync_ReturnsOk()
        {
            var discussionId = new Guid("8077167c-d724-4258-8451-b617dc4bdfec");
            string userNameIncreasedBy = "TestUserName";
            using StringContent jsonContent = new(JsonSerializer.Serialize(new { discussionId, userNameIncreasedBy }),
                Encoding.UTF8, "application/json");
            var response = await client.PatchAsync("api/Discussion/IncreaseDiscussionRatingByOne", jsonContent);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DecreaseDiscussionRatingByOneAsync_ReturnsBadRequest()
        {
            Guid discussionId = Guid.NewGuid();
            string userNameDecreasedBy = "TestUserName";
            using StringContent jsonContent = new(JsonSerializer.Serialize(new { discussionId, userNameDecreasedBy }),
                Encoding.UTF8, "application/json");
            var response = await client.PatchAsync("api/Discussion/DecreaseDiscussionRatingByOne", jsonContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DecreaseDiscussionRatingByOneAsync_ReturnsOk()
        {
            var discussionId = new Guid("8077167c-d724-4258-8451-b617dc4bdfec");
            string userNameDecreasedBy = "TestUserName";
            using StringContent jsonContent = new(JsonSerializer.Serialize(new { discussionId, userNameDecreasedBy }),
                Encoding.UTF8, "application/json");
            var response = await client.PatchAsync("api/Discussion/DecreaseDiscussionRatingByOne", jsonContent);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreateDiscussionAsync_ReturnsOkWithCreatedDiscussionId()
        {
            var model = new CreateDiscussionDto
            {
                Content = "TestContentX", CreatedBy = "TestCreatedByX", Title = "TestTitleX",
                TopicName = "TestTopicNameX"
            };
            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Discussion/CreateDiscussion", jsonContent);

            response.EnsureSuccessStatusCode();
            var createdDiscussionId = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.NotNull(createdDiscussionId);
        }
    }
}
