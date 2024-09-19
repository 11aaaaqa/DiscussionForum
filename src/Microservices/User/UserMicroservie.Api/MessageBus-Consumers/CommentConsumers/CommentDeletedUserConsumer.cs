using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers.CommentConsumers
{
    public class CommentDeletedUserConsumer : IConsumer<ICommentDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public CommentDeletedUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ICommentDeleted> context)
        {
            var user = await databaseContext.Users.SingleAsync(x => x.UserName == context.Message.CommentCreatedBy);
            user.CommentsIds.Remove(context.Message.CommentId);
            user.Answers -= 1;
            databaseContext.Users.Update(user);
            await databaseContext.SaveChangesAsync();
        }
    }
}
