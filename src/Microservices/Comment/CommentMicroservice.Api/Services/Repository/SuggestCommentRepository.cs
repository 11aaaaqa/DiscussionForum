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

        public async Task<List<SuggestedComment>> GetByDiscussionIdAsync(Guid id) =>
            await context.SuggestedComments.Where(x => x.DiscussionId == id).ToListAsync();

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
            context.SuggestedComments.Remove(new SuggestedComment { Id = id });
            await context.SaveChangesAsync();
        }
    }
}
