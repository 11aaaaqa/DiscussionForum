using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class DiscussionDeletedUserConsumer : IConsumer<IDiscussionDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public DiscussionDeletedUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IDiscussionDeleted> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x =>
                x.UserName == context.Message.UserNameDiscussionCreatedBy);
            if (user is not null)
            {
                user.Posts -= 1;
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
