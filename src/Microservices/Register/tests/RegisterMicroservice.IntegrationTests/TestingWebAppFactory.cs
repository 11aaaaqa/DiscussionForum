using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RegisterMicroservice.Api.Database;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;

namespace RegisterMicroservice.IntegrationTests
{
    public class TestingWebAppFactory : WebApplicationFactory<Program>
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

                services.AddTransient<ITokenService, TokenService>();
                services.AddScoped<IEmailSender, EmailSender>();
                services.AddTransient<IUserService, UserService>();

                services.AddDbContext<ApplicationDbContext>(x =>
                    x.UseInMemoryDatabase("RegisterMicroserviceInMemoryDb"));

                var sp = services.BuildServiceProvider();
                using var appContext = sp.GetRequiredService<ApplicationDbContext>();
                appContext.Database.EnsureCreated();

                //todo: add users
                appContext.SaveChanges();
            });
        }
    }
}
