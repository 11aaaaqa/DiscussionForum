using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class UserWithUnconfirmedEmailDeletedUserConsumer : IConsumer<IUserWithUnconfirmedEmailDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserWithUnconfirmedEmailDeletedUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext  = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserWithUnconfirmedEmailDeleted> context)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(x =>
                x.AspNetUserId == context.Message.AspNetUserId);
            if (user is not null)
            {
                databaseContext.Users.Remove(user);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
