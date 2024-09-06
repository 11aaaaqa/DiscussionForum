using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers.CommentConsumers
{
    public class CommentCreatedConsumer : IConsumer<ICommentCreated>
    {
        private readonly ApplicationDbContext databaseContext;

        public CommentCreatedConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ICommentCreated> context)
        {
            var user = await databaseContext.Users.SingleAsync(x => x.UserName == context.Message.CreatedBy);
            user.CommentsIds.Add(context.Message.CommentId);
            databaseContext.Users.Update(user);
            await databaseContext.SaveChangesAsync();
        }
    }
}
