using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DiscussionMicroservice.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var response = await client.GetAsync($"/api/Discussion/GetDiscussionById?id={Guid.Empty}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
