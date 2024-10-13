using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class SuggestedCommentAcceptedUserConsumer : IConsumer<ISuggestedCommentAccepted>
    {
        private readonly ApplicationDbContext databaseContext;

        public SuggestedCommentAcceptedUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ISuggestedCommentAccepted> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x => x.UserName == context.Message.CreatedBy);
            if (user is not null)
            {
                user.Answers += 1;
                databaseContext.Users.Update(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
