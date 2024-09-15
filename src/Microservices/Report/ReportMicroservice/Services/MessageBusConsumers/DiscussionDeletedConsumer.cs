using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;

namespace ReportMicroservice.Api.Services.MessageBusConsumers
{
    public class DiscussionDeletedConsumer : IConsumer<IDiscussionDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public DiscussionDeletedConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IDiscussionDeleted> context)
        {
            var reports = await databaseContext.Reports
                .Where(x => x.ReportedDiscussionId == context.Message.DiscussionId).ToListAsync();
            foreach (var report in reports)
            {
                databaseContext.Reports.Remove(report);
            }

            await databaseContext.SaveChangesAsync();
        }
    }
}
