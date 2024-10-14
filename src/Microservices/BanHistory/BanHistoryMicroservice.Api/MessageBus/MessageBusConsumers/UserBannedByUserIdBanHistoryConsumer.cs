using BanHistoryMicroservice.Api.Models;
using BanHistoryMicroservice.Api.Services;
using MassTransit;
using MessageBus.Messages.BanMessages;

namespace BanHistoryMicroservice.Api.MessageBus.MessageBusConsumers
{
    public class UserBannedByUserIdBanHistoryConsumer : IConsumer<IUserBannedByUserId>
    {
        private readonly IBanService<Ban> banService;

        public UserBannedByUserIdBanHistoryConsumer(IBanService<Ban> banService)
        {
           this.banService = banService;
        }
        public async Task Consume(ConsumeContext<IUserBannedByUserId> context)
        {
            await banService.CreateAsync(new Ban
            {
                Id = Guid.NewGuid(), BanType = context.Message.BanType, DurationInDays = context.Message.DurationIdDays,
                Reason = context.Message.Reason, UserId = context.Message.UserId, UserName = context.Message.UserName, BannedBy = context.Message.BannedBy,
                BannedFrom = DateTime.UtcNow, BannedUntil = DateTime.UtcNow.AddDays(context.Message.DurationIdDays)
            });
        }
    }
}
