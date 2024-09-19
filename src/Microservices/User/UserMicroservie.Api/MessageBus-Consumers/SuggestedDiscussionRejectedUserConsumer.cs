using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class SuggestedDiscussionRejectedUserConsumer : IConsumer<ISuggestedDiscussionRejected>
    {
        private readonly ApplicationDbContext databaseContext;

        public SuggestedDiscussionRejectedUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ISuggestedDiscussionRejected> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.SuggestedBy);
            if (user is not null)
            {
                user.SuggestedDiscussionsIds.Remove(context.Message.SuggestedDiscussionId);
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
