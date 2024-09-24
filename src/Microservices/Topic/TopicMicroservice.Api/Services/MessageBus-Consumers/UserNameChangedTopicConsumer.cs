using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;

namespace TopicMicroservice.Api.Services.MessageBus_Consumers
{
    public class UserNameChangedTopicConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedTopicConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var suggestedTopicsWithOldUserName = await databaseContext.SuggestedTopics.
                Where(x => x.SuggestedBy == context.Message.OldUserName).ToListAsync();
            foreach (var suggestedTopicWithOldUserName in suggestedTopicsWithOldUserName)
            {
                suggestedTopicWithOldUserName.SuggestedBy = context.Message.NewUserName;
            }
            await databaseContext.SaveChangesAsync();
        }
    }
}
