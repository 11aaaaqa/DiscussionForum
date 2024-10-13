using MassTransit;
using MessageBus.Messages;
using UserMicroservice.Api.Database;
using UserMicroservice.Api.Models;

namespace UserMicroservice.Api.MessageBus_Consumers
{
    public class UserRegisteredUserConsumer : IConsumer<IUserRegistered>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserRegisteredUserConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserRegistered> context)
        {
            await databaseContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                UserName = context.Message.UserName,
                RegisteredAt = DateOnly.FromDateTime(DateTime.UtcNow),
                Posts = 0,
                Answers = 0,
                AspNetUserId = context.Message.UserId,
                BanType = "Not banned",
                BannedFor = "Not banned",
                BannedUntil = new DateTime(),
                IsBanned = false
            });
            await databaseContext.SaveChangesAsync();
        }
    }
}
