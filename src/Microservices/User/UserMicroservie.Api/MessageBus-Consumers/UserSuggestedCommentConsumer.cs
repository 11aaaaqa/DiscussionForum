using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class UserSuggestedCommentConsumer : IConsumer<IUserSuggestedComment>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserSuggestedCommentConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserSuggestedComment> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.SuggestedBy);
            if (user is not null)
            {
                user.SuggestedCommentsIds.Add(context.Message.SuggestedCommentId);
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
