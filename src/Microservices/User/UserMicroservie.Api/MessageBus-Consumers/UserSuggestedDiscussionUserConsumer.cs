using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class UserSuggestedDiscussionUserConsumer : IConsumer<IUserSuggestedDiscussion>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserSuggestedDiscussionUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserSuggestedDiscussion> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.CreatedBy);
            if (user is not null)
            {
                user.SuggestedDiscussionsIds.Add(context.Message.SuggestedDiscussionId);
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
