using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TopicMicroservice.Api.DTOs;
using TopicMicroservice.Api.Models;

namespace TopicMicroservice.IntegrationTests
{
    public class ControllersIntegrationTests : IClassFixture<TestingWebApplicationFactory<Program>>
    {
        private readonly HttpClient client;
        public ControllersIntegrationTests(TestingWebApplicationFactory<Program> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task SuggestTopicAsync_ReturnsBadRequest()
        {
            var model = new TopicDto { Name = "TestName1" };
            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/SuggestTopic/SuggestTopic", jsonContent);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseModel = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            Assert.NotNull(responseModel);
            Assert.Equal("Тема уже предложена", responseModel.Reason);
        }

        [Fact]
        public async Task SuggestTopicAsync_ReturnsOk()
        {
            var model = new TopicDto { Name = "CorrectTopicName" };
            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/SuggestTopic/SuggestTopic", jsonContent);
            response.EnsureSuccessStatusCode();
            var getResponse = await client.GetAsync("api/SuggestTopic/GetAllSuggestedTopics");
            getResponse.EnsureSuccessStatusCode();
            var suggestedTopics = await getResponse.Content.ReadFromJsonAsync<List<SuggestedTopic>>();
            Assert.NotNull(suggestedTopics);
            var createdSuggestedTopic = suggestedTopics.SingleOrDefault(x => x.Name == "CorrectTopicName");
            Assert.NotNull(createdSuggestedTopic);
        }

        [Fact]
        public async Task RejectSuggestedTopicAsync_ReturnsOk()
        {
            var id = new Guid("e7982376-cd20-4e49-9045-c247f8d0b8ed");
            var response = await client.DeleteAsync($"api/SuggestTopic/RejectSuggestedTopic/{id}");
            
            response.EnsureSuccessStatusCode();
            var getResponse = await client.GetAsync("api/SuggestTopic/GetAllSuggestedTopics");
            getResponse.EnsureSuccessStatusCode();
            var suggestedTopics = await getResponse.Content.ReadFromJsonAsync<List<SuggestedTopic>>();
            Assert.NotNull(suggestedTopics);
            var isContains = suggestedTopics.Contains(new SuggestedTopic { Id = id });
            Assert.False(isContains);
        }

        [Fact]
        public async Task AcceptSuggestedTopicAsync_ReturnsOk()
        {
            var id = new Guid("0d603044-db37-4220-874f-684cc96c4dc0");
            var response = await client.DeleteAsync($"api/SuggestTopic/AcceptSuggestedTopic/{id}");

            response.EnsureSuccessStatusCode();

            var getSuggestedTopicResponse = await client.GetAsync("api/SuggestTopic/GetAllSuggestedTopics");
            getSuggestedTopicResponse.EnsureSuccessStatusCode();
            var suggestedTopics = await getSuggestedTopicResponse.Content.ReadFromJsonAsync<List<SuggestedTopic>>();
            Assert.NotNull(suggestedTopics);
            var isContains = suggestedTopics.Contains(new SuggestedTopic { Id = id });
            Assert.False(isContains);

            var getTopicResponse = await client.GetAsync($"api/Topic/GetById?id={id}");
            getTopicResponse.EnsureSuccessStatusCode();
        }
    }
}
