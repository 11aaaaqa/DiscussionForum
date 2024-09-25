using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.Database;

namespace RegisterMicroservice.Api.MessageBus.Consumers
{
    public class UserNameChangedRegisterConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedRegisterConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var userWithOldUserName = await databaseContext.Users.SingleAsync(x => x.UserName == context.Message.OldUserName);
            userWithOldUserName.UserName = context.Message.NewUserName;
            userWithOldUserName.NormalizedUserName = context.Message.NewUserName.ToUpper();
            await databaseContext.SaveChangesAsync();
        }
    }
}
