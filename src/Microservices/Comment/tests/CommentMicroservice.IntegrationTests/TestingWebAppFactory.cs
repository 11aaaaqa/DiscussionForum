using CommentMicroservice.Api.Database;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services.Repository;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommentMicroservice.IntegrationTests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
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

                services.AddTransient<IRepository<SuggestedComment>, SuggestCommentRepository>();
                services.AddTransient<IRepository<Comment>, CommentRepository>();

                services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("CommentMicroserviceTesting"));
                services.AddMassTransitTestHarness();

                var sp = services.BuildServiceProvider();
                using var appContext = sp.GetRequiredService<ApplicationDbContext>();
                appContext.Database.EnsureCreated();

                appContext.SuggestedComments.AddAsync(new SuggestedComment { Content = "TestContent1", CreatedBy = "TestCreatedBy1", CreatedDate = DateTime.UtcNow, DiscussionId = new Guid("746710db-da1b-4fb4-99c4-f4c77a51ed3c"), Id = new Guid("6551f496-0ebf-4417-89c8-a8de47579923") });
                appContext.SuggestedComments.AddAsync(new SuggestedComment { Content = "TestContent2", CreatedBy = "TestCreatedBy2", CreatedDate = DateTime.UtcNow, DiscussionId = new Guid("661646ad-1872-4ddb-88e2-ee96ea65f488"), Id = new Guid("67e7243e-cf1b-411b-bd73-7bb63be07d01") });
                appContext.SuggestedComments.AddAsync(new SuggestedComment { Content = "TestContent3", CreatedBy = "TestCreatedBy3", CreatedDate = DateTime.UtcNow, DiscussionId = new Guid("a89aab75-d7f6-4902-9b62-97ae87b74c0c"), Id = new Guid("dc0ab4e5-50a8-46cb-9626-ce978e970d84") });
                appContext.SaveChanges();
            });
        }
    }
}
