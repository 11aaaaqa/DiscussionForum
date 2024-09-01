using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using MassTransit.Transports.Fabric;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TopicMicroservice.Api.Database;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services.MessageBus_Consumers;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.IntegrationTests
{
    public class OtherIntegrationTests
    {
        //[Fact]
        //public async Task MassTransitTest()
        //{
        //    await using var provider = new ServiceCollection()
        //        .AddLogging(x => x.Services.AddSingleton<ILoggerFactory, LoggerFactory>())
        //        .AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("MassTransitTest"))
        //        .AddTransient<IRepository<Topic>, TopicRepository>()
        //        .AddMassTransitTestHarness(x =>
        //        {
        //            x.AddConsumer<DiscussionAddedConsumer>();
        //        })
        //        .BuildServiceProvider(true);
        //    using var scope = provider.CreateScope();
        //    var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //    appContext.Database.EnsureCreated();
        //    appContext.Topics.Add(new Topic { Id = Guid.NewGuid(), Name = "TestTopicName", PostsCount = 0 });
        //    appContext.SaveChanges();

        //    var harness = provider.GetRequiredService<ITestHarness>();

        //    await harness.Start();

        //    var client = harness.GetRequestClient<IDiscussionAdded>();

        //    var response = await client.GetResponse<IDiscussionAdded>(new { TopicName = "TestTopicName" });

        //    Assert.NotNull(response.Message);


        //    await harness.Stop();
        //    Assert.True(await harness.Sent.Any<IDiscussionAdded>());
        //    Assert.True(await harness.Consumed.Any<DiscussionAddedConsumer>());
        //}
    }
}
