using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;

namespace ReportMicroservice.Api.MessageBus.Consumers
{
    public class UserNameChangedReportConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedReportConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var reportsWithOldUserName = await databaseContext.Reports.Where(x => x.UserNameReportedBy == context.Message.OldUserName)
                .ToListAsync();
            foreach (var reportWithOldUserName in reportsWithOldUserName)
            {
                reportWithOldUserName.UserNameReportedBy = context.Message.NewUserName;
            }

            await databaseContext.SaveChangesAsync();
        }
    }
}
