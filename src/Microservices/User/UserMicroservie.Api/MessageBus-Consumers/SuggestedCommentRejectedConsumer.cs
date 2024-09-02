using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class SuggestedCommentRejectedConsumer : IConsumer<ISuggestedCommentRejected>
    {
        private readonly ApplicationDbContext databaseContext;

        public SuggestedCommentRejectedConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ISuggestedCommentRejected> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.CreatedBy);
            if (user is not null)
            {
                user.SuggestedCommentsIds.Remove(context.Message.AcceptedCommentId);
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
