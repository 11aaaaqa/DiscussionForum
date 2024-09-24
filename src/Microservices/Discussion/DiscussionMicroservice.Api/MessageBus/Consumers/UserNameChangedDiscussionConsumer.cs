using DiscussionMicroservice.Api.Database;
using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.MessageBus.Consumers
{
    public class UserNameChangedDiscussionConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedDiscussionConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var discussionsWithOldCreatedByUserName = await databaseContext.Discussions
                .Where(x => x.CreatedBy == context.Message.OldUserName).ToListAsync();
            foreach (var discussionWithOldCreatedByUserName in discussionsWithOldCreatedByUserName)
            {
                discussionWithOldCreatedByUserName.CreatedBy = context.Message.NewUserName;
            }

            var suggestedDiscussionsWithOldCreatedByUserName = await databaseContext.SuggestedDiscussions
                .Where(x => x.CreatedBy == context.Message.OldUserName).ToListAsync();
            foreach (var suggestedDiscussionWithOldCreatedByUserName in suggestedDiscussionsWithOldCreatedByUserName)
            {
                suggestedDiscussionWithOldCreatedByUserName.CreatedBy = context.Message.NewUserName;
            }

            await databaseContext.SaveChangesAsync();
        }
    }
}
