using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TopicMicroservice.Api.Database;

namespace TopicMicroservice.IntegrationTests
{
    public class TestingWebApplicationFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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
            });
        }
    }
}
