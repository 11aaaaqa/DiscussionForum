using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;

namespace TopicMicroservice.Api.Services.MessageBus_Consumers
{
    public class DiscussionDeletedConsumer : IConsumer<IDiscussionDeletedForTopic>
    {
        private readonly ApplicationDbContext databaseContext;

        public DiscussionDeletedConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task Consume(ConsumeContext<IDiscussionDeletedForTopic> context)
        {
            var topic = await databaseContext.Topics.SingleOrDefaultAsync(x => x.Name == context.Message.TopicName);
            if (topic is not null)
            {
                topic.PostsCount -= 1;
                databaseContext.Topics.Update(topic);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
