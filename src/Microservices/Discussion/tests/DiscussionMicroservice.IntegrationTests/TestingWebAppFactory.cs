using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
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
                appContext.Discussions.Add(new Discussion { Content = "TestContent2", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy2", Id = Guid.NewGuid(), Rating = -224, TopicName = "TestTopicName", Title = "TestTitle2" });
                appContext.Discussions.Add(new Discussion { Content = "TestContent3", CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), CreatedBy = "TestCreatedBy3", Id = Guid.NewGuid(), Rating = 1, TopicName = "TestTopicName2", Title = "TestTitle3" });
                


                appContext.SaveChanges();
            });
        }
    }
}
