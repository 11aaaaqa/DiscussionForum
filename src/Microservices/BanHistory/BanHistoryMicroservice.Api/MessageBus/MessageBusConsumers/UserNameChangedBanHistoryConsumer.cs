using BanHistoryMicroservice.Api.Database;
using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;

namespace BanHistoryMicroservice.Api.MessageBus.MessageBusConsumers
{
    public class UserNameChangedBanHistoryConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedBanHistoryConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var bansWithOldUserName = await databaseContext.Bans.Where(x => x.UserName == context.Message.OldUserName)
                .ToListAsync();
            foreach (var banWithOldUserName in bansWithOldUserName)
            {
                banWithOldUserName.UserName = context.Message.NewUserName;
            }

            await databaseContext.SaveChangesAsync();
        }
    }
}
