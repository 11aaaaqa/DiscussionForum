using MassTransit;
using MessageBus.Messages;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.Api.Services.MessageBus_Consumers
{
    public class DiscussionDeletedConsumer : IConsumer<IDiscussionDeleted>
    {
        private readonly IRepository<Topic> topicRepository;

        public DiscussionDeletedConsumer(IRepository<Topic> topicRepository)
        {
            this.topicRepository = topicRepository;
        }
        public async Task Consume(ConsumeContext<IDiscussionDeleted> context)
        {
            var topic = await topicRepository.GetByNameAsync(context.Message.TopicName);
            if (topic is not null)
            {
                topic.PostsCount -= 1;
                await topicRepository.UpdateAsync(topic);
            }
        }
    }
}
