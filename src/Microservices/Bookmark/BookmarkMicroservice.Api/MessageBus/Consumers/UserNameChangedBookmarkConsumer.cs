using BookmarkMicroservice.Api.Database;
using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.MessageBus.Consumers
{
    public class UserNameChangedBookmarkConsumer : IConsumer<IUserNameChanged>
    {
        private readonly ApplicationDbContext databaseContext;

        public UserNameChangedBookmarkConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<IUserNameChanged> context)
        {
            var bookmarks = await databaseContext.Bookmarks.Where(x => x.UserName == context.Message.OldUserName)
                .ToListAsync();
            foreach (var bookmark in bookmarks)
            {
                bookmark.UserName = context.Message.NewUserName;
            }
            await databaseContext.SaveChangesAsync();
        }
    }
}
