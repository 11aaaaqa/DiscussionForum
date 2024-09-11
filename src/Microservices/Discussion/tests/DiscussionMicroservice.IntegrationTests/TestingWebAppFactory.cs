using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscussionMicroservice.IntegrationTests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(x => x.ClearProviders());
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddMassTransitTestHarness();
                services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("DiscussionControllerTests"));
                
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                using var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                try
                {
                    appContext.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    throw new Exception("Database wasn't created");
                }

                appContext.Discussions.Add(new Discussion { Content = "TestContent", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy", Id = new Guid("8077167c-d724-4258-8451-b617dc4bdfec"), Rating = 123, TopicName = "TestTopicName", Title = "TestTitle" });
                appContext.Discussions.Add(new Discussion { Content = "TestContent2", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy2", Id = new Guid("5a38b400-8be8-466c-a0ed-62249bc7811b"), Rating = -224, TopicName = "TestTopicName", Title = "TestTitle2" });
                appContext.Discussions.Add(new Discussion { Content = "TestContent3", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy3", Id = Guid.NewGuid(), Rating = 1, TopicName = "TestTopicName2", Title = "TestTitle3" });

                appContext.SuggestedDiscussions.Add(new SuggestedDiscussion { Id = new Guid("1fd24884-7dd7-40a9-bfe5-f8247a5e3bc7"), Content = "TestContent555", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy555", Rating = 523, Title = "TestTitle555", TopicName = "TestTopicName555"});
                appContext.SuggestedDiscussions.Add(new SuggestedDiscussion { Id = new Guid("0629b192-d453-4dc2-86f5-d8bbd7fb0955"), Content = "TestContent666", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy666", Rating = 343, Title = "TestTitle666", TopicName = "TestTopicName666" });
                appContext.SuggestedDiscussions.Add(new SuggestedDiscussion { Id = new Guid("8bb9b1c2-4112-4b9c-b67b-8775a0ca6584"), Content = "TestContent565", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy565", Rating = 343, Title = "TestTitle565", TopicName = "TestTopicName565" });

                appContext.SaveChanges();
            });
        }
    }
}
