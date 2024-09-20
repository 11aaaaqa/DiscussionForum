using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TopicMicroservice.Api.Database;
using TopicMicroservice.Api.Models;

namespace TopicMicroservice.IntegrationTests
{
    public class TestingWebApplicationFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(x => x.ClearProviders());
            builder.ConfigureServices(services =>
            {
                var descriptor =
                    services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("TestTopicMicroservice"));

                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                using var databaseContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    databaseContext.Database.EnsureCreated();
                }
                catch (Exception e)
                {
                    throw new Exception("Database wasn't created");
                }

                databaseContext.SuggestedTopics.Add(new SuggestedTopic { Id = new Guid("e7982376-cd20-4e49-9045-c247f8d0b8ed"), Name = "TestName1", PostsCount = 123, SuggestedBy = "TestSuggestedBy1" });
                databaseContext.SuggestedTopics.Add(new SuggestedTopic { Id = new Guid("0d603044-db37-4220-874f-684cc96c4dc0"), Name = "TestName2", PostsCount = 321, SuggestedBy = "TestSuggestedBy1" });
                databaseContext.SuggestedTopics.Add(new SuggestedTopic { Id = new Guid("ac7dabc5-ef1d-47e7-aedd-871bbd1054a0"), Name = "TestName3", PostsCount = 0, SuggestedBy = "TestSuggestedBy3" });

                databaseContext.SaveChanges();
            });
        }
    }
}
