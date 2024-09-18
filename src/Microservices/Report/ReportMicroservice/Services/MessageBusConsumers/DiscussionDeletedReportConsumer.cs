using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;

namespace ReportMicroservice.Api.Services.MessageBusConsumers
{
    public class DiscussionDeletedReportConsumer : IConsumer<IDiscussionDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public DiscussionDeletedReportConsumer(ApplicationDbContext databaseContext)
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
