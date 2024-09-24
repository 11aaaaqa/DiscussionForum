using CommentMicroservice.Api.Database;
using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;

namespace CommentMicroservice.Api.MessageBus.Consumers
{
    public class UserNameChangedCommentConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedCommentConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var commentsWithOldUserName = await databaseContext.Comments
                .Where(x => x.CreatedBy == context.Message.OldUserName).ToListAsync();
            foreach (var commentWithOldUserName in commentsWithOldUserName)
            {
                commentWithOldUserName.CreatedBy = context.Message.NewUserName;
            }

            var suggestedCommentsWithOldUserName = await databaseContext.SuggestedComments
                .Where(x => x.CreatedBy == context.Message.OldUserName).ToListAsync();
            foreach (var suggestedCommentWithOldUserName in suggestedCommentsWithOldUserName)
            {
                suggestedCommentWithOldUserName.CreatedBy = context.Message.NewUserName;
            }

            await databaseContext.SaveChangesAsync();
        }
    }
}
