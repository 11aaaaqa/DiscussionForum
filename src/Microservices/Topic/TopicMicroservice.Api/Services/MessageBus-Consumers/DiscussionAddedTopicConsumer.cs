using MassTransit;
using MessageBus.Messages;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.Api.Services.MessageBus_Consumers
{
    public class DiscussionAddedTopicConsumer : IConsumer<ISuggestedDiscussionAccepted>
    {
        private readonly IRepository<Topic> topicRepository;

        public DiscussionAddedTopicConsumer(IRepository<Topic> topicRepository)
        {
            this.topicRepository = topicRepository;
        }
        public async Task Consume(ConsumeContext<ISuggestedDiscussionAccepted> context)
        {
            var topic = await topicRepository.GetByNameAsync(context.Message.TopicName);
            topic.PostsCount += 1;
            await topicRepository.UpdateAsync(topic);
        }
    }
}
