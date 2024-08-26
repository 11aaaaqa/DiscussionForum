using MassTransit;
using MessageBus.Messages;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.Api.Services.MessageBus_Consumers
{
    public class DiscussionAddedConsumer : IConsumer<IDiscussionAdded>
    {
        private readonly IRepository<Topic> topicRepository;

        public DiscussionAddedConsumer(IRepository<Topic> topicRepository)
        {
            this.topicRepository = topicRepository;
        }
        public async Task Consume(ConsumeContext<IDiscussionAdded> context)
        {
            var topic = await topicRepository.GetByNameAsync(context.Message.TopicName);
            topic.PostsCount += context.Message.DiscussionCount;
            await topicRepository.UpdateAsync(topic);
        }
    }
}
