using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommentMicroservice.Api.DTOs;
using CommentMicroservice.Api.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
