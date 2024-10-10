using CommentMicroservice.Api.Database;
using CommentMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentMicroservice.Api.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly ApplicationDbContext context;

        public PaginationService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> DoesNextCommentsPageExistAsync(CommentParameters commentParameters)
        {
            int totalCommentsCount = await context.Comments.CountAsync();
            int totalRequestedCommentsCount = commentParameters.PageSize * commentParameters.PageNumber;
            int startRequestedCommentsCount = totalRequestedCommentsCount - commentParameters.PageSize;
            return (totalCommentsCount > startRequestedCommentsCount);
        }

        public async Task<bool> DoesNextSuggestedCommentsPageExistAsync(CommentParameters commentParameters)
        {
            int totalSuggestedCommentsCount = await context.SuggestedComments.CountAsync();
            int totalRequestedSuggestedCommentsCount = commentParameters.PageSize * commentParameters.PageNumber;
            int startRequestedSuggestedCommentsCount = totalRequestedSuggestedCommentsCount - commentParameters.PageSize;
            return (totalSuggestedCommentsCount > startRequestedSuggestedCommentsCount);
        }

        public async Task<bool> DoesNextCommentsByDiscussionIdPageExistAsync(CommentParameters commentParameters, Guid discussionsId)
        {
            int totalSuggestedCommentsCount = await context.Comments.Where(x => x.DiscussionId == discussionsId).CountAsync();
            int totalRequestedSuggestedCommentsCount = commentParameters.PageSize * commentParameters.PageNumber;
            int startRequestedSuggestedCommentsCount = totalRequestedSuggestedCommentsCount - commentParameters.PageSize;
            return (totalSuggestedCommentsCount > startRequestedSuggestedCommentsCount);
        }
    }
}
