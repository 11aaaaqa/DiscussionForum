using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class SuggestedCommentAcceptedConsumer : IConsumer<ISuggestedCommentAccepted>
    {
        private readonly ApplicationDbContext databaseContext;

        public SuggestedCommentAcceptedConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ISuggestedCommentAccepted> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.CreatedBy);
            if (user is not null)
            {
                user.SuggestedCommentsIds.Remove(context.Message.AcceptedCommentId);
                user.CommentsIds.Add(context.Message.AcceptedCommentId);
                user.Answers += 1;
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
