using CommentMicroservice.Api.Database;
using CommentMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentMicroservice.Api.Services.Repository
{
    public class SuggestCommentRepository : IRepository<SuggestedComment>
    {
        private readonly ApplicationDbContext context;

        public SuggestCommentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<SuggestedComment>> GetAllAsync(CommentParameters commentParameters)
        {
            var comments = await context.SuggestedComments
                .Skip(commentParameters.PageSize * (commentParameters.PageNumber - 1))
                .Take(commentParameters.PageSize)
                .ToListAsync();
            return comments;
        } 

        public async Task<List<SuggestedComment>> GetByDiscussionIdAsync(Guid id, CommentParameters commentParameters)
        {
            var comments = await context.SuggestedComments.Where(x => x.DiscussionId == id)
                .Skip(commentParameters.PageSize * (commentParameters.PageNumber - 1))
                .Take(commentParameters.PageSize)
                .ToListAsync();
            return comments;
        }

        public async Task<List<SuggestedComment>> GetByIds(params Guid[] ids)
        {
            var suggestedComments = new List<SuggestedComment>();
            foreach (var id in ids)
            {
                var suggestedComment = await context.SuggestedComments.SingleOrDefaultAsync(x => x.Id == id);
                if (suggestedComment is not null) 
                    suggestedComments.Add(suggestedComment);
            }
            return suggestedComments;
        }

        public async Task<List<SuggestedComment>> GetByUserName(string userName)
            => await context.SuggestedComments.Where(x => x.CreatedBy == userName).ToListAsync();


        public async Task<SuggestedComment?> GetByIdAsync(Guid id) =>
            await context.SuggestedComments.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<SuggestedComment> UpdateAsync(SuggestedComment model)
        {
            context.SuggestedComments.Update(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<SuggestedComment> CreateAsync(SuggestedComment model)
        {
            await context.SuggestedComments.AddAsync(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var suggestedComment = await context.SuggestedComments.SingleOrDefaultAsync(x => x.Id == id);
            if (suggestedComment is not null)
            {
                context.SuggestedComments.Remove(suggestedComment);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserNameAsync(string userName)
        {
            var suggestedComments = await context.SuggestedComments.Where(x => x.CreatedBy == userName).ToListAsync();
            foreach (var suggestedComment in suggestedComments)
            {
                context.SuggestedComments.Remove(suggestedComment);
            }

            await context.SaveChangesAsync();
        }
    }
}
