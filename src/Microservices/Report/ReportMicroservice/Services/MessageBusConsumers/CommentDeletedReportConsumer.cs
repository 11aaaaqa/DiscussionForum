using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;

namespace ReportMicroservice.Api.Services.MessageBusConsumers
{
    public class CommentDeletedReportConsumer : IConsumer<ICommentDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public CommentDeletedReportConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ICommentDeleted> context)
        {
            var reports = await databaseContext.Reports.Where(x => x.ReportedCommentId == context.Message.CommentId).ToListAsync();
            foreach (var report in reports)
            {
                databaseContext.Reports.Remove(report);
            }
            await databaseContext.SaveChangesAsync();
        }
    }
}
