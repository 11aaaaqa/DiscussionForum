using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;

namespace TopicMicroservice.Api.Services.MessageBus_Consumers
{
    public class DiscussionDeletedTopicConsumer : IConsumer<IDiscussionDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public DiscussionDeletedTopicConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task Consume(ConsumeContext<IDiscussionDeleted> context)
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
