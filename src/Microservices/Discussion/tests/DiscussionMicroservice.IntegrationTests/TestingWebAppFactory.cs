using DiscussionMicroservice.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace DiscussionMicroservice.IntegrationTests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

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
            });
        }
    }
}
