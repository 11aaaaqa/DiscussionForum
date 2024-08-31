using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DiscussionMicroservice.Api.Models;

namespace DiscussionMicroservice.IntegrationTests
{
    public class SuggestDiscussionControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient client;

        public SuggestDiscussionControllerIntegrationTests(TestingWebAppFactory<Program> factory)
        {
            client = factory.CreateClient();
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
        public async Task GetAllSuggestedDiscussionsAsync_ReturnOkWithListOfSuggestedDiscussions()
        {
            var response = await client.GetAsync("api/SuggestDiscussion/GetAllSuggestedDiscussions");

            response.EnsureSuccessStatusCode();
            var suggestedDiscussions = await response.Content.ReadFromJsonAsync<List<SuggestedDiscussion>>();
            Assert.NotNull(suggestedDiscussions);
            Assert.Equal(2, suggestedDiscussions.Count);
        }
    }
}
