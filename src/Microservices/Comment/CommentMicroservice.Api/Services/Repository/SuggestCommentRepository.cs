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

        public async Task<List<SuggestedComment>> GetAllAsync() => await context.SuggestedComments.ToListAsync();

        public async Task<List<SuggestedComment>?> GetByDiscussionIdAsync(Guid id)
        {
            var suggestedDiscussion = await context.SuggestedComments.FirstOrDefaultAsync(x => x.DiscussionId == id);
            if (suggestedDiscussion is null)
            {
                return null;
            }
            return await context.SuggestedComments.Where(x => x.DiscussionId == id).ToListAsync();
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
    }
}
