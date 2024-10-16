using CommentMicroservice.Api.Database;
using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;

namespace CommentMicroservice.Api.MessageBus.Consumers
{
    public class CommentDeletedCommentConsumer : IConsumer<ICommentDeleted>
    {
        private readonly ApplicationDbContext databaseContext;

        public CommentDeletedCommentConsumer(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task Consume(ConsumeContext<ICommentDeleted> context)
        {
            var comments = await databaseContext.Comments.Where(x => x.RepliedOnCommentId == context.Message.CommentId).ToListAsync();
            var suggestedComments = await databaseContext.SuggestedComments
                .Where(x => x.RepliedOnCommentId == context.Message.CommentId).ToListAsync();

            foreach (var comment in comments)
            {
                comment.RepliedOnCommentId = null;
                comment.RepliedOnCommentContent = null;
            }

            foreach (var suggestedComment in suggestedComments)
            {
                suggestedComment.RepliedOnCommentId = null;
                suggestedComment.RepliedOnCommentContent = null;
            }

            await databaseContext.SaveChangesAsync();
        }
    }
}
