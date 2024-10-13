using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class SuggestedDiscussionAcceptedUSerConsumer : IConsumer<ISuggestedDiscussionAccepted>
    {
        private readonly ApplicationDbContext databaseContext;

        public SuggestedDiscussionAcceptedUSerConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ISuggestedDiscussionAccepted> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.CreatedBy);
            if (user is not null)
            {
                user.Posts += 1;
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
