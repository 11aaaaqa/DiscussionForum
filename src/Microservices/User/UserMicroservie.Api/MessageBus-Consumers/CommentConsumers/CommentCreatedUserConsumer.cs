using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers.CommentConsumers
{
    public class CommentCreatedUserConsumer : IConsumer<ICommentCreated>
    {
        private readonly ApplicationDbContext databaseContext;

        public CommentCreatedUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ICommentCreated> context)
        {
            var user = await databaseContext.Users.SingleAsync(x => x.UserName == context.Message.CreatedBy);
            user.Answers += 1;
            databaseContext.Users.Update(user);
            await databaseContext.SaveChangesAsync();
        }
    }
}
