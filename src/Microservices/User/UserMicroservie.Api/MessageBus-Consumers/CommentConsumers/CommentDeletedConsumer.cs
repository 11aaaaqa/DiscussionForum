using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers.CommentConsumers
{
    public class CommentDeletedConsumer : IConsumer<ICommentDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public CommentDeletedConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ICommentDeleted> context)
        {
            var user = await databaseContext.Users.SingleAsync(x => x.UserName == context.Message.CommentCreatedBy);
            user.CommentsIds.Remove(context.Message.CommentId);
            databaseContext.Users.Update(user);
            await databaseContext.SaveChangesAsync();
        }
    }
}
